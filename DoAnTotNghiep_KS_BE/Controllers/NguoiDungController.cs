using DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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

            // Không cho phép tự thay đổi vai trò của chính mình
            if (id == currentUserId && updateDTO.VaiTro != null)
            {
                return BadRequest(new { message = "Không thể tự thay đổi vai trò của chính mình" });
            }

            // Không cho phép tự khóa tài khoản của chính mình
            if (id == currentUserId && updateDTO.TrangThai == "Tạm khóa")
            {
                return BadRequest(new { message = "Không thể tự khóa tài khoản của chính mình" });
            }

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
    }
}