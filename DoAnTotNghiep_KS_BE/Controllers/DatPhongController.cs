using DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatPhongController : ControllerBase
    {
        private readonly IDatPhongRepository _datPhongRepository;

        public DatPhongController(IDatPhongRepository datPhongRepository)
        {
            _datPhongRepository = datPhongRepository;
        }

        // GET: api/DatPhong/KiemTraThongTin
        [HttpGet("KiemTraThongTin")]
        [Authorize]
        public async Task<IActionResult> KiemTraThongTinNguoiDung()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var result = await _datPhongRepository.KiemTraThongTinNguoiDungAsync(userId);

                return Ok(new
                {
                    success = result.DayDuThongTin,
                    data = result,
                    message = result.Message
                });
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

        // POST: api/DatPhong/KiemTraPhongTrong
        [HttpPost("KiemTraPhongTrong")]
        public async Task<IActionResult> KiemTraPhongTrong([FromBody] KiemTraPhongTrongRequest request)
        {
            try
            {
                if (request.NgayNhanPhong.Date < DateTime.Now.Date)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ngày nhận phòng phải từ hôm nay trở đi"
                    });
                }

                if (request.NgayTraPhong.Date <= request.NgayNhanPhong.Date)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ngày trả phòng phải sau ngày nhận phòng"
                    });
                }

                var phongBiTrung = await _datPhongRepository.KiemTraPhongTrongAsync(
                    request.DanhSachMaPhong,
                    request.NgayNhanPhong,
                    request.NgayTraPhong
                );

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        phongKhaDung = request.DanhSachMaPhong.Except(phongBiTrung).ToList(),
                        phongBiTrung = phongBiTrung
                    },
                    message = phongBiTrung.Any()
                        ? $"Có {phongBiTrung.Count} phòng đã được đặt"
                        : "Tất cả phòng đều khả dụng"
                });
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

        // POST: api/DatPhong
        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> CreateDatPhong([FromBody] CreateDatPhongDTO createDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var (success, message, maDatPhong) = await _datPhongRepository.CreateDatPhongAsync(userId, createDTO);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new
                {
                    success = true,
                    data = new { maDatPhong },
                    message
                });
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

        // GET: api/DatPhong/CuaToi
        [HttpGet("CuaToi")]
        [Authorize]
        public async Task<IActionResult> GetDatPhongCuaToi()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var datPhongs = await _datPhongRepository.GetDatPhongByKhachHangAsync(userId);

                return Ok(new
                {
                    success = true,
                    data = datPhongs,
                    message = $"Tìm thấy {datPhongs.Count} đặt phòng"
                });
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

        // GET: api/DatPhong/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDatPhongById(int id)
        {
            try
            {
                var datPhong = await _datPhongRepository.GetDatPhongByIdAsync(id);
                if (datPhong == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy đặt phòng"
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "LeTan")
                {
                    if (string.IsNullOrEmpty(userIdClaim) ||
                        !int.TryParse(userIdClaim, out int userId) ||
                        datPhong.MaKhachHang != userId)
                    {
                        return Forbid();
                    }
                }

                return Ok(new
                {
                    success = true,
                    data = datPhong
                });
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

        // PUT: api/DatPhong/{id}/Huy
        [HttpPut("{id}/Huy")]
        [Authorize]
        public async Task<IActionResult> HuyDatPhong(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var (success, message) = await _datPhongRepository.HuyDatPhongAsync(id, userId);

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

        // GET: api/DatPhong (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> GetAllDatPhong()
        {
            try
            {
                var datPhongs = await _datPhongRepository.GetAllDatPhongAsync();

                return Ok(new
                {
                    success = true,
                    data = datPhongs,
                    message = $"Tìm thấy {datPhongs.Count} đặt phòng"
                });
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

        // POST: api/DatPhong/TrucTiep (Lễ tân tạo đặt phòng cho khách walk-in)
        [HttpPost("TrucTiep")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> CreateDatPhongTrucTiep([FromBody] CreateDatPhongTrucTiepDTO createDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int leTanId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var (success, message, data) = await _datPhongRepository.CreateDatPhongTrucTiepAsync(leTanId, createDTO);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new
                {
                    success = true,
                    data,
                    message
                });
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

        // PUT: api/DatPhong/{id}/CheckIn
        [HttpPut("{id}/CheckIn")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> CheckIn(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int leTanId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var (success, message) = await _datPhongRepository.CheckInAsync(id, leTanId);

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

        // PUT: api/DatPhong/{id}/CheckOut
        [HttpPut("{id}/CheckOut")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> CheckOut(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int leTanId))
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var (success, message) = await _datPhongRepository.CheckOutAsync(id, leTanId);

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

    // Request model
    public class KiemTraPhongTrongRequest
    {
        public List<int> DanhSachMaPhong { get; set; } = new();
        public DateTime NgayNhanPhong { get; set; }
        public DateTime NgayTraPhong { get; set; }
    }
}