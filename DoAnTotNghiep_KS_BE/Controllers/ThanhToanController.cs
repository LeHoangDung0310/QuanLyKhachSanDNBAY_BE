using DoAnTotNghiep_KS_BE.Interfaces.dto.ThanhToan;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IThanhToanRepository _thanhToanRepository;

        public ThanhToanController(IThanhToanRepository thanhToanRepository)
        {
            _thanhToanRepository = thanhToanRepository;
        }

        // POST: api/ThanhToan
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateThanhToan([FromBody] CreateThanhToanDTO createDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var (success, message, data) = await _thanhToanRepository.CreateThanhToanAsync(createDTO, userId);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new { success = true, message, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        // GET: api/ThanhToan/DatPhong/{maDatPhong}
        [HttpGet("DatPhong/{maDatPhong}")]
        [Authorize]
        public async Task<IActionResult> GetThongTinThanhToan(int maDatPhong)
        {
            try
            {
                var data = await _thanhToanRepository.GetThongTinThanhToanAsync(maDatPhong);

                if (data == null)
                {
                    return NotFound(new { success = false, message = "Đặt phòng không tồn tại" });
                }

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        // GET: api/ThanhToan/LichSu/{maDatPhong}
        [HttpGet("LichSu/{maDatPhong}")]
        [Authorize]
        public async Task<IActionResult> GetLichSuThanhToan(int maDatPhong)
        {
            try
            {
                var data = await _thanhToanRepository.GetLichSuThanhToanAsync(maDatPhong);

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        // PUT: api/ThanhToan/{id}/XacNhan
        [HttpPut("{id}/XacNhan")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> XacNhanThanhToan(int id, [FromBody] XacNhanThanhToanRequest request)
        {
            try
            {
                var (success, message) = await _thanhToanRepository.XacNhanThanhToanOnlineAsync(id, request.TransactionId ?? "");

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }

        // DELETE: api/ThanhToan/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> HuyThanhToan(int id)
        {
            try
            {
                var (success, message) = await _thanhToanRepository.HuyThanhToanAsync(id);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server: " + ex.Message
                });
            }
        }
    }

    // Request Models
    public class XacNhanThanhToanRequest
    {
        public string? TransactionId { get; set; }
    }
}