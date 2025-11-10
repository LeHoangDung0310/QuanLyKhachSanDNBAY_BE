using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HinhAnhPhongController : ControllerBase
    {
        private readonly IHinhAnhPhongRepository _hinhAnhPhongRepository;
        private readonly IPhongRepository _phongRepository;

        public HinhAnhPhongController(
            IHinhAnhPhongRepository hinhAnhPhongRepository,
            IPhongRepository phongRepository)
        {
            _hinhAnhPhongRepository = hinhAnhPhongRepository;
            _phongRepository = phongRepository;
        }

        // GET: api/HinhAnhPhong
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HinhAnhPhongDTO>>> GetHinhAnhPhongs()
        {
            var hinhAnhs = await _hinhAnhPhongRepository.GetAllHinhAnhPhongsAsync();
            return Ok(new
            {
                success = true,
                data = hinhAnhs,
                total = hinhAnhs.Count()
            });
        }

        // GET: api/HinhAnhPhong/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchHinhAnhPhongs(
            [FromQuery] int? maPhong,
            [FromQuery] string? soPhong,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate phân trang
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDTO = new SearchHinhAnhPhongDTO
            {
                MaPhong = maPhong,
                SoPhong = soPhong,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _hinhAnhPhongRepository.SearchHinhAnhPhongsAsync(searchDTO);

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

        // GET: api/HinhAnhPhong/Phong/5
        [HttpGet("Phong/{maPhong}")]
        public async Task<ActionResult<IEnumerable<HinhAnhPhongDTO>>> GetHinhAnhsByPhong(int maPhong)
        {
            var hinhAnhs = await _hinhAnhPhongRepository.GetHinhAnhsByPhongIdAsync(maPhong);
            return Ok(new
            {
                success = true,
                data = hinhAnhs,
                total = hinhAnhs.Count()
            });
        }

        // GET: api/HinhAnhPhong/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HinhAnhPhongDTO>> GetHinhAnhPhong(int id)
        {
            var hinhAnh = await _hinhAnhPhongRepository.GetHinhAnhPhongByIdAsync(id);
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

        // POST: api/HinhAnhPhong
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HinhAnhPhongDTO>> CreateHinhAnhPhong(CreateHinhAnhPhongDTO createHinhAnhPhongDTO)
        {
            // Kiểm tra phòng có tồn tại không
            if (!await _phongRepository.PhongExistsAsync(createHinhAnhPhongDTO.MaPhong))
            {
                return BadRequest(new { message = "Phòng không tồn tại" });
            }

            var hinhAnhPhong = await _hinhAnhPhongRepository.CreateHinhAnhPhongAsync(createHinhAnhPhongDTO);
            var hinhAnhPhongDTO = await _hinhAnhPhongRepository.GetHinhAnhPhongByIdAsync(hinhAnhPhong.MaHinhAnh);

            return CreatedAtAction(nameof(GetHinhAnhPhong), new { id = hinhAnhPhong.MaHinhAnh }, new
            {
                success = true,
                message = "Thêm hình ảnh thành công",
                data = hinhAnhPhongDTO
            });
        }

        // PUT: api/HinhAnhPhong/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHinhAnhPhong(int id, UpdateHinhAnhPhongDTO updateHinhAnhPhongDTO)
        {
            var result = await _hinhAnhPhongRepository.UpdateHinhAnhPhongAsync(id, updateHinhAnhPhongDTO);
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

        // DELETE: api/HinhAnhPhong/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHinhAnhPhong(int id)
        {
            var result = await _hinhAnhPhongRepository.DeleteHinhAnhPhongAsync(id);
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