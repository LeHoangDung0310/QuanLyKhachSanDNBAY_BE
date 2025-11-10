using DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TienNghiController : ControllerBase
    {
        private readonly ITienNghiRepository _tienNghiRepository;

        public TienNghiController(ITienNghiRepository tienNghiRepository)
        {
            _tienNghiRepository = tienNghiRepository;
        }

        // GET: api/TienNghi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TienNghiDTO>>> GetTienNghis()
        {
            var tienNghis = await _tienNghiRepository.GetAllTienNghisAsync();
            return Ok(new
            {
                success = true,
                data = tienNghis,
                total = tienNghis.Count()
            });
        }

        // GET: api/TienNghi/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchTienNghis(
            [FromQuery] string? ten,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate phân trang
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDTO = new SearchTienNghiDTO
            {
                Ten = ten,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _tienNghiRepository.SearchTienNghisAsync(searchDTO);

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

        // GET: api/TienNghi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TienNghiDTO>> GetTienNghi(int id)
        {
            var tienNghi = await _tienNghiRepository.GetTienNghiByIdAsync(id);
            if (tienNghi == null)
            {
                return NotFound(new { message = "Không tìm thấy tiện nghi" });
            }
            return Ok(new
            {
                success = true,
                data = tienNghi
            });
        }

        // POST: api/TienNghi
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TienNghiDTO>> CreateTienNghi(CreateTienNghiDTO createTienNghiDTO)
        {
            var tienNghi = await _tienNghiRepository.CreateTienNghiAsync(createTienNghiDTO);
            var tienNghiDTO = await _tienNghiRepository.GetTienNghiByIdAsync(tienNghi.MaTienNghi);

            return CreatedAtAction(nameof(GetTienNghi), new { id = tienNghi.MaTienNghi }, new
            {
                success = true,
                message = "Tạo tiện nghi thành công",
                data = tienNghiDTO
            });
        }

        // PUT: api/TienNghi/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTienNghi(int id, UpdateTienNghiDTO updateTienNghiDTO)
        {
            var result = await _tienNghiRepository.UpdateTienNghiAsync(id, updateTienNghiDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy tiện nghi" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật tiện nghi thành công"
            });
        }

        // DELETE: api/TienNghi/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTienNghi(int id)
        {
            var result = await _tienNghiRepository.DeleteTienNghiAsync(id);
            if (!result)
            {
                return BadRequest(new { message = "Không thể xóa tiện nghi (đang được sử dụng hoặc không tồn tại)" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa tiện nghi thành công"
            });
        }
    }
}