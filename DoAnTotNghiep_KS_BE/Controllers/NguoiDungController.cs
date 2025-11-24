using DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungRepository _nguoiDungRepository;

        public NguoiDungController(INguoiDungRepository nguoiDungRepository)
        {
            _nguoiDungRepository = nguoiDungRepository;
        }

        // GET: api/NguoiDung
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetNguoiDungs()
        {
            var nguoiDungs = await _nguoiDungRepository.GetAllNguoiDungsAsync();
            return Ok(new
            {
                success = true,
                data = nguoiDungs,
                total = nguoiDungs.Count()
            });
        }

        // GET: api/NguoiDung/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchNguoiDungs(
            [FromQuery] string? searchTerm,
            [FromQuery] string? vaiTro,
            [FromQuery] string? trangThai,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // KHÔNG validate vaiTro nữa, để repo tự lọc

            // Validate trạng thái nếu có
            if (!string.IsNullOrWhiteSpace(trangThai) &&
                !new[] { "Hoạt động", "Tạm khóa" }.Contains(trangThai))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ" });
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDto = new SearchNguoiDungDTO
            {
                SearchTerm = searchTerm,
                VaiTro = vaiTro,
                TrangThai = trangThai,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _nguoiDungRepository.SearchNguoiDungsAsync(searchDto);

            return Ok(new
            {
                data,
                pagination = new
                {
                    currentPage = pageNumber,
                    pageSize,
                    totalItems = total,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                }
            });
        }

        // GET: api/NguoiDung/Role/{vaiTro}
        [HttpGet("Role/{vaiTro}")]
        public async Task<ActionResult<IEnumerable<NguoiDungDTO>>> GetNguoiDungsByRole(string vaiTro)
        {
            var roleNorm = vaiTro.Trim();

            var allowedRoles = new[] { "Admin", "KhachHang", "LeTan" };
            if (!allowedRoles.Any(r => string.Equals(r, roleNorm, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { message = "Vai trò không hợp lệ" });
            }

            if (roleNorm.Equals("admin", StringComparison.OrdinalIgnoreCase))
                vaiTro = "Admin";
            else if (roleNorm.Equals("khachhang", StringComparison.OrdinalIgnoreCase))
                vaiTro = "KhachHang";
            else if (roleNorm.Equals("letan", StringComparison.OrdinalIgnoreCase))
                vaiTro = "LeTan";

            var nguoiDungs = await _nguoiDungRepository.GetNguoiDungsByRoleAsync(vaiTro);
            return Ok(new
            {
                success = true,
                data = nguoiDungs,
                total = nguoiDungs.Count()
            });
        }

        // GET: api/NguoiDung/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NguoiDungDTO>> GetNguoiDung(int id)
        {
            var nguoiDung = await _nguoiDungRepository.GetNguoiDungByIdAsync(id);
            if (nguoiDung == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(new
            {
                success = true,
                data = nguoiDung
            });
        }

        // PUT: api/NguoiDung/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNguoiDung(int id, UpdateNguoiDungAdminDTO updateDTO)
        {
            // Lấy thông tin người dùng hiện tại từ token
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null)
            {
                return Unauthorized(new { message = "Không xác định được người dùng" });
            }

            var currentUserId = int.Parse(currentUserIdClaim.Value);

            // Nếu admin đang chỉnh sửa CHÍNH MÌNH
            if (id == currentUserId)
            {
                // Chặn đổi vai trò của chính mình
                if (!string.IsNullOrWhiteSpace(updateDTO.VaiTro))
                {
                    return BadRequest(new { message = "Không thể tự thay đổi vai trò của chính mình" });
                }

                // Chặn tự khóa tài khoản của chính mình
                if (string.Equals(updateDTO.TrangThai, "Tạm khóa", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Không thể tự khóa tài khoản của chính mình" });
                }
            }

            // Với người khác thì cho đổi thoải mái (trong repo sẽ update theo giá trị có trong DTO)
            var result = await _nguoiDungRepository.UpdateNguoiDungAsync(id, updateDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật người dùng thành công"
            });
        }

        // DELETE: api/NguoiDung/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoiDung(int id)
        {
            // Lấy thông tin người dùng hiện tại từ token
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim == null)
            {
                return Unauthorized(new { message = "Không xác định được người dùng" });
            }

            var currentUserId = int.Parse(currentUserIdClaim.Value);

            // Không cho phép tự xóa tài khoản của chính mình
            if (id == currentUserId)
            {
                return BadRequest(new { message = "Không thể xóa tài khoản của chính mình" });
            }

            // Kiểm tra có thể xóa không
            var canDelete = await _nguoiDungRepository.CanDeleteNguoiDungAsync(id);
            if (!canDelete)
            {
                return BadRequest(new
                {
                    message = "Không thể xóa người dùng này vì có dữ liệu liên quan (đặt phòng, đánh giá, ...)"
                });
            }

            var result = await _nguoiDungRepository.DeleteNguoiDungAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa người dùng thành công"
            });
        }

        // GET: api/NguoiDung/Statistics
        [HttpGet("Statistics")]
        public async Task<ActionResult> GetStatistics()
        {
            var allUsers = await _nguoiDungRepository.GetAllNguoiDungsAsync();

            var statistics = new
            {
                total = allUsers.Count(),
                admins = allUsers.Count(u => u.VaiTro == "Admin"),
                customers = allUsers.Count(u => u.VaiTro == "KhachHang"),
                employees = allUsers.Count(u => u.VaiTro == "LeTan"),
                active = allUsers.Count(u => u.TrangThai == "Hoạt động"),
                locked = allUsers.Count(u => u.TrangThai == "Tạm khóa")
            };

            return Ok(new
            {
                success = true,
                data = statistics
            });
        }

        // GET: api/NguoiDung/Profile/Me
        [HttpGet("Profile/Me")]
        [Authorize]
        public async Task<ActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không xác thực được người dùng" });
            }

            var nguoiDung = await _nguoiDungRepository.GetNguoiDungByIdAsync(userId);
            if (nguoiDung == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng" });
            }

            return Ok(new { success = true, data = nguoiDung });
        }

        // PUT: api/NguoiDung/Profile/Me
        [HttpPut("Profile/Me")]
        [Authorize]
        public async Task<ActionResult> UpdateMyProfile([FromBody] UpdateProfileDTO updateDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không xác thực được người dùng" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            var result = await _nguoiDungRepository.UpdateProfileAsync(userId, updateDTO);
            if (!result)
            {
                return BadRequest(new { message = "Cập nhật thông tin thất bại" });
            }

            return Ok(new { success = true, message = "Cập nhật thông tin thành công" });
        }

        // PUT: api/NguoiDung/Profile/ChangePassword
        [HttpPut("Profile/ChangePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không xác thực được người dùng" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            var (success, message) = await _nguoiDungRepository.ChangePasswordAsync(userId, changePasswordDTO);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { success = true, message });
        }

        // POST: api/NguoiDung/Profile/UploadAvatar
        [HttpPost("Profile/UploadAvatar")]
        [Authorize]
        public async Task<ActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không xác thực được người dùng" });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng chọn file ảnh" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)" });
            }

            // Validate file size (5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "Kích thước file tối đa 5MB" });
            }

            try
            {
                // Tạo thư mục lưu ảnh nếu chưa có
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file unique
                var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh vào database
                var avatarUrl = $"/uploads/avatars/{fileName}";
                var result = await _nguoiDungRepository.UpdateAvatarAsync(userId, avatarUrl);

                if (!result)
                {
                    // Xóa file nếu update DB thất bại
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return BadRequest(new { message = "Cập nhật ảnh đại diện thất bại" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Upload ảnh đại diện thành công",
                    avatarUrl = avatarUrl
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading avatar: {ex.Message}");
                return StatusCode(500, new { message = "Có lỗi xảy ra khi upload ảnh" });
            }
        }
    }
}