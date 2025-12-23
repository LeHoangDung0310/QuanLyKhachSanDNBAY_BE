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
        public async Task<IActionResult> SearchPhongs(
        [FromQuery] SearchPhongDTO searchDTO,
        [FromQuery] DateTime? ngayNhanPhong,
        [FromQuery] DateTime? ngayTraPhong)
        {
            // ✅ KHÔNG CHỌN NGÀY → HÔM NAY
            var start = ngayNhanPhong?.Date ?? DateTime.Today;
            var end = ngayTraPhong?.Date ?? DateTime.Today.AddDays(1);

            var (data, total) = await _phongRepository.SearchPhongsAsync(
                searchDTO,
                start,
                end
            );

            return Ok(new
            {
                success = true,
                data,
                total
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