using DoAnTotNghiep_KS_BE.Interfaces.dto.Tang;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TangController : ControllerBase
    {
        private readonly ITangRepository _tangRepository;

        public TangController(ITangRepository tangRepository)
        {
            _tangRepository = tangRepository;
        }

        // GET: api/Tang
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TangDTO>>> GetTangs()
        {
            var tangs = await _tangRepository.GetAllTangsAsync();
            return Ok(new
            {
                success = true,
                data = tangs,
                total = tangs.Count()
            });
        }

        // GET: api/Tang/Search
        [HttpGet("Search")]
        public async Task<ActionResult> SearchTangs(
            [FromQuery] string? tenTang,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate phân trang
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var searchDTO = new SearchTangDTO
            {
                TenTang = tenTang,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var (data, total) = await _tangRepository.SearchTangsAsync(searchDTO);

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

        // GET: api/Tang/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TangDTO>> GetTang(int id)
        {
            var tang = await _tangRepository.GetTangByIdAsync(id);
            if (tang == null)
            {
                return NotFound(new { message = "Không tìm thấy tầng" });
            }
            return Ok(new
            {
                success = true,
                data = tang
            });
        }

        // POST: api/Tang
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TangDTO>> CreateTang(CreateTangDTO createTangDTO)
        {
            var tang = await _tangRepository.CreateTangAsync(createTangDTO);
            var tangDTO = await _tangRepository.GetTangByIdAsync(tang.MaTang);

            return CreatedAtAction(nameof(GetTang), new { id = tang.MaTang }, new
            {
                success = true,
                message = "Tạo tầng thành công",
                data = tangDTO
            });
        }

        // PUT: api/Tang/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTang(int id, UpdateTangDTO updateTangDTO)
        {
            var result = await _tangRepository.UpdateTangAsync(id, updateTangDTO);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy tầng" });
            }

            return Ok(new
            {
                success = true,
                message = "Cập nhật tầng thành công"
            });
        }

        // DELETE: api/Tang/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTang(int id)
        {
            var result = await _tangRepository.DeleteTangAsync(id);
            if (!result)
            {
                return BadRequest(new { message = "Không thể xóa tầng (còn phòng hoặc không tồn tại)" });
            }

            return Ok(new
            {
                success = true,
                message = "Xóa tầng thành công"
            });
        }
    }
}