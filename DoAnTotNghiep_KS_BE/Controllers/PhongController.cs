using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Interfaces.dto.Phong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongRepository _phongRepository;
        private readonly MyDbContext _context;

        public PhongController(IPhongRepository phongRepository, MyDbContext context)
        {
            _phongRepository = phongRepository;
            _context = context;
        }

        // GET: api/Phong
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhongDTO>>> GetPhongs()
        {
            var phongs = await _phongRepository.GetAllPhongsAsync();
            return Ok(new
            {
                success = true,
                data = phongs,
                total = phongs.Count()
            });
        }

        // GET: api/Phong/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchPhongs(
            [FromQuery] string? soPhong,
            [FromQuery] int? maLoaiPhong,
            [FromQuery] int? soGiuongMin,
            [FromQuery] int? soGiuongMax,
            [FromQuery] int? soNguoiToiDaMin,
            [FromQuery] int? soNguoiToiDaMax,
            [FromQuery] string? trangThai,
            [FromQuery] int? maTang,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate trạng thái nếu có
            if (!string.IsNullOrWhiteSpace(trangThai) &&
                !new[] { "Trong", "DaDat", "DangSuDung", "BaoTri" }.Contains(trangThai))
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ" });
            }

            // Validate phân trang
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            // Validate khoảng giá trị
            if (soGiuongMin.HasValue && soGiuongMin.Value < 0)
            {
                return BadRequest(new { message = "Số giường tối thiểu phải >= 0" });
            }
            if (soGiuongMax.HasValue && soGiuongMin.HasValue && soGiuongMax.Value < soGiuongMin.Value)
            {
                return BadRequest(new { message = "Số giường tối đa phải >= số giường tối thiểu" });
            }

            if (soNguoiToiDaMin.HasValue && soNguoiToiDaMin.Value < 0)
            {
                return BadRequest(new { message = "Số người tối đa tối thiểu phải >= 0" });
            }
            if (soNguoiToiDaMax.HasValue && soNguoiToiDaMin.HasValue && soNguoiToiDaMax.Value < soNguoiToiDaMin.Value)
            {
                return BadRequest(new { message = "Số người tối đa tối đa phải >= số người tối đa tối thiểu" });
            }

            var searchDTO = new SearchPhongDTO
            {
                SoPhong = soPhong,
                MaLoaiPhong = maLoaiPhong,
                SoGiuongMin = soGiuongMin,
                SoGiuongMax = soGiuongMax,
                SoNguoiToiDaMin = soNguoiToiDaMin,
                SoNguoiToiDaMax = soNguoiToiDaMax,
                TrangThai = trangThai,
                MaTang = maTang,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _phongRepository.SearchPhongsAsync(searchDTO);

            return Ok(new
            {
                success = true,
                data = data,
                pagination = new
                {
                    currentPage = pageNumber,
                    pageSize = pageSize,
                    totalItems = total,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                }
            });
        }

        // GET: api/Phong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhongDTO>> GetPhong(int id)
        {
            var phong = await _phongRepository.GetPhongByIdAsync(id);
            if (phong == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }
            return Ok(new
            {
                success = true,
                data = phong
            });
        }

        // POST: api/Phong
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PhongDTO>> CreatePhong(CreatePhongDTO createPhongDTO)
        {
            // Kiểm tra số phòng đã tồn tại
            if (await _phongRepository.SoPhongExistsAsync(createPhongDTO.SoPhong))
            {
                return BadRequest(new { message = "Số phòng đã tồn tại" });
            }

            var phong = await _phongRepository.CreatePhongAsync(createPhongDTO);
            var phongDTO = await _phongRepository.GetPhongByIdAsync(phong.MaPhong);

            return CreatedAtAction(nameof(GetPhong), new { id = phong.MaPhong }, new
            {
                success = true,
                message = "Tạo phòng thành công",
                data = phongDTO
            });
        }

        // PUT: api/Phong/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePhong(int id, UpdatePhongDTO updatePhongDTO)
        {
            var result = await _phongRepository.UpdatePhongAsync(id, updatePhongDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật phòng thành công"
            });
        }

        // DELETE: api/Phong/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhong(int id)
        {
            var result = await _phongRepository.DeletePhongAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy phòng" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa phòng thành công"
            });
        }

        // ✅ NEW ENDPOINT: GET: api/Phong/PhongTrong
        [HttpGet("PhongTrong")]
        public async Task<IActionResult> GetPhongTrong(
            [FromQuery] DateTime ngayNhanPhong,
            [FromQuery] DateTime ngayTraPhong)
        {
            try
            {
                // Validate input dates
                if (ngayNhanPhong == default || ngayTraPhong == default)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng cung cấp ngày nhận phòng và ngày trả phòng"
                    });
                }

                if (ngayTraPhong <= ngayNhanPhong)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ngày trả phòng phải sau ngày nhận phòng"
                    });
                }

                // Get available rooms from repository
                var phongKhaDung = await _phongRepository.GetPhongTrongAsync(ngayNhanPhong, ngayTraPhong);

                return Ok(new
                {
                    success = true,
                    data = phongKhaDung,
                    message = $"Tìm thấy {phongKhaDung.Count()} phòng trống",
                    total = phongKhaDung.Count()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi tải danh sách phòng trống",
                    error = ex.Message
                });
            }
        }
    }
}