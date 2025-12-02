using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HinhAnhLPhongController : ControllerBase
    {
        private readonly IHinhAnhLPhongRepository _hinhAnhLPhongRepository;
        private readonly ILoaiPhongRepository _loaiPhongRepository;

        public HinhAnhLPhongController(
            IHinhAnhLPhongRepository hinhAnhLPhongRepository,
            ILoaiPhongRepository loaiPhongRepository)
        {
            _hinhAnhLPhongRepository = hinhAnhLPhongRepository;
            _loaiPhongRepository = loaiPhongRepository;
        }

        // GET: api/HinhAnhLPhong
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HinhAnhLPhongDTO>>> GetHinhAnhLPhongs()
        {
            var hinhAnhs = await _hinhAnhLPhongRepository.GetAllHinhAnhLPhongsAsync();
            return Ok(new
            {
                success = true,
                data = hinhAnhs,
                total = hinhAnhs.Count()
            });
        }

        // GET: api/HinhAnhLPhong/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchHinhAnhLPhongs(
            [FromQuery] int? maLoaiPhong,
            [FromQuery] string? tenLoaiPhong,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDTO = new SearchHinhAnhLPhongDTO
            {
                MaLoaiPhong = maLoaiPhong,
                TenLoaiPhong = tenLoaiPhong,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _hinhAnhLPhongRepository.SearchHinhAnhLPhongsAsync(searchDTO);

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

        // GET: api/HinhAnhLPhong/LoaiPhong/5
        [HttpGet("LoaiPhong/{maLoaiPhong}")]
        public async Task<ActionResult<IEnumerable<HinhAnhLPhongDTO>>> GetHinhAnhsByLoaiPhong(int maLoaiPhong)
        {
            var hinhAnhs = await _hinhAnhLPhongRepository.GetHinhAnhsByLoaiPhongIdAsync(maLoaiPhong);
            return Ok(new
            {
                success = true,
                data = hinhAnhs,
                total = hinhAnhs.Count()
            });
        }

        // GET: api/HinhAnhLPhong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HinhAnhLPhongDTO>> GetHinhAnhLPhong(int id)
        {
            var hinhAnh = await _hinhAnhLPhongRepository.GetHinhAnhLPhongByIdAsync(id);
            if (hinhAnh == null)
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh" });
            }
            return Ok(new
            {
                success = true,
                data = hinhAnh
            });
        }

        // POST: api/HinhAnhLPhong
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HinhAnhLPhongDTO>> CreateHinhAnhLPhong([FromForm] CreateHinhAnhLPhongDTO createHinhAnhLPhongDTO)
        {
            if (!await _loaiPhongRepository.LoaiPhongExistsAsync(createHinhAnhLPhongDTO.MaLoaiPhong))
            {
                return BadRequest(new { message = "Loại phòng không tồn tại" });
            }

            if (createHinhAnhLPhongDTO.File == null || createHinhAnhLPhongDTO.File.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng chọn file hình ảnh" });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(createHinhAnhLPhongDTO.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif, .webp)" });
            }

            if (createHinhAnhLPhongDTO.File.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "Kích thước file không được vượt quá 5MB" });
            }

            try
            {
                var hinhAnhLPhong = await _hinhAnhLPhongRepository.CreateHinhAnhLPhongAsync(createHinhAnhLPhongDTO);
                var hinhAnhLPhongDTO = await _hinhAnhLPhongRepository.GetHinhAnhLPhongByIdAsync(hinhAnhLPhong.MaHinhAnh);

                return CreatedAtAction(nameof(GetHinhAnhLPhong), new { id = hinhAnhLPhong.MaHinhAnh }, new
                {
                    success = true,
                    message = "Thêm hình ảnh thành công",
                    data = hinhAnhLPhongDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi upload hình ảnh: " + ex.Message });
            }
        }

        // PUT: api/HinhAnhLPhong/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHinhAnhLPhong(int id, UpdateHinhAnhLPhongDTO updateHinhAnhLPhongDTO)
        {
            var result = await _hinhAnhLPhongRepository.UpdateHinhAnhLPhongAsync(id, updateHinhAnhLPhongDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật hình ảnh thành công"
            });
        }

        // DELETE: api/HinhAnhLPhong/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHinhAnhLPhong(int id)
        {
            var result = await _hinhAnhLPhongRepository.DeleteHinhAnhLPhongAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa hình ảnh thành công"
            });
        }
    }
}