using DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiPhongController : ControllerBase
    {
        private readonly ILoaiPhongRepository _loaiPhongRepository;

        public LoaiPhongController(ILoaiPhongRepository loaiPhongRepository)
        {
            _loaiPhongRepository = loaiPhongRepository;
        }

        // GET: api/LoaiPhong - THÊM PHƯƠNG THỨC NÀY
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoaiPhongDTO>>> GetLoaiPhongs()
        {
            var loaiPhongs = await _loaiPhongRepository.GetAllLoaiPhongsAsync();
            return Ok(new
            {
                success = true,
                data = loaiPhongs,
                total = loaiPhongs.Count()
            });
        }

        // GET: api/LoaiPhong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoaiPhongDTO>> GetLoaiPhong(int id)
        {
            var loaiPhong = await _loaiPhongRepository.GetLoaiPhongByIdAsync(id);
            if (loaiPhong == null)
            {
                return NotFound(new { message = "Không tìm thấy loại phòng" });
            }
            return Ok(new
            {
                success = true,
                data = loaiPhong
            });
        }

        // GET: api/LoaiPhong/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchLoaiPhongs(
            [FromQuery] string? tenLoaiPhong,
            [FromQuery] int? soNguoiToiDaMin,
            [FromQuery] int? soNguoiToiDaMax,
            [FromQuery] int? soGiuongMin,
            [FromQuery] int? soGiuongMax,
            [FromQuery] decimal? giaMin,
            [FromQuery] decimal? giaMax,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDTO = new SearchLoaiPhongDTO
            {
                TenLoaiPhong = tenLoaiPhong,
                SoNguoiToiDaMin = soNguoiToiDaMin,
                SoNguoiToiDaMax = soNguoiToiDaMax,
                SoGiuongMin = soGiuongMin,
                SoGiuongMax = soGiuongMax,
                GiaMin = giaMin,
                GiaMax = giaMax,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _loaiPhongRepository.SearchLoaiPhongsAsync(searchDTO);

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

        // POST: api/LoaiPhong
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LoaiPhongDTO>> CreateLoaiPhong(CreateLoaiPhongDTO createLoaiPhongDTO)
        {
            var loaiPhong = await _loaiPhongRepository.CreateLoaiPhongAsync(createLoaiPhongDTO);
            var loaiPhongDTO = await _loaiPhongRepository.GetLoaiPhongByIdAsync(loaiPhong.MaLoaiPhong);

            return CreatedAtAction(nameof(GetLoaiPhong), new { id = loaiPhong.MaLoaiPhong }, new
            {
                success = true,
                message = "Thêm loại phòng thành công",
                data = loaiPhongDTO
            });
        }

        // PUT: api/LoaiPhong/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLoaiPhong(int id, UpdateLoaiPhongDTO updateLoaiPhongDTO)
        {
            var result = await _loaiPhongRepository.UpdateLoaiPhongAsync(id, updateLoaiPhongDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy loại phòng" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật loại phòng thành công"
            });
        }

        // DELETE: api/LoaiPhong/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLoaiPhong(int id)
        {
            var result = await _loaiPhongRepository.DeleteLoaiPhongAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy loại phòng" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa loại phòng thành công"
            });
        }
    }
}