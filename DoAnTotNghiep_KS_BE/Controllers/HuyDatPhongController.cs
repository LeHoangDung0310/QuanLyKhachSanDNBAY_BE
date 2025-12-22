using DoAnTotNghiep_KS_BE.Interfaces.dto.HuyDatPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HuyDatPhongController : ControllerBase
    {
        private readonly IHuyDatPhongRepository _huyDatPhongRepository;

        public HuyDatPhongController(IHuyDatPhongRepository huyDatPhongRepository)
        {
            _huyDatPhongRepository = huyDatPhongRepository;
        }

        // ✅ KIỂM TRA ĐIỀU KIỆN HỦY (giữ nguyên)
        [HttpGet("KiemTraDieuKien/{maDatPhong}")]
        [Authorize]
        public async Task<IActionResult> KiemTraDieuKienHuy(int maDatPhong)
        {
            var (canCancel, message, phiGiu, tienHoan) = await _huyDatPhongRepository.KiemTraDieuKienHuyAsync(maDatPhong);

            return Ok(new
            {
                success = canCancel,
                message,
                data = new
                {
                    canCancel,
                    phiGiu,
                    tienHoan
                }
            });
        }

        // ✅ NGƯỜI DÙNG YÊU CẦU HỦY (CẬP NHẬT - kèm thông tin ngân hàng)
        [HttpPost("YeuCauHuy/{maDatPhong}")]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> YeuCauHuyDatPhong(int maDatPhong, [FromBody] YeuCauHuyRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var (success, message, phiGiu) = await _huyDatPhongRepository.YeuCauHuyDatPhongAsync(
                maDatPhong,
                request.LyDo,
                userId,
                request.NganHang,
                request.SoTaiKhoan,
                request.TenChuTK
            );

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new
            {
                success = true,
                message,
                data = new { phiGiu }
            });
        }

        // ✅ HỦY ĐẶT PHÒNG SAU CHECK-IN (chỉ trong ngày đầu tiên)
        [HttpPost("HuySauCheckIn/{maDatPhong}")]
        [Authorize(Roles = "LeTan")]
        public async Task<IActionResult> HuySauCheckIn(int maDatPhong)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }
            bool isLeTan = roleClaim != null && (roleClaim.Contains("LeTan") || roleClaim.Contains("Admin"));
            var (success, message, phiGiu, tienHoan, khachHang, phongList) = await _huyDatPhongRepository.HuySauCheckInAsync(
                maDatPhong,
                userId,
                isLeTan
            );
            if (!success)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new
            {
                success = true,
                message,
                data = new { phiGiu, tienHoan, khachHang, phongList }
            });
        }

        // ✅ LẤY TẤT CẢ YÊU CẦU HỦY (LỄ TÂN/ADMIN)
        [HttpGet]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _huyDatPhongRepository.GetAllAsync();
            return Ok(new { success = true, data = list });
        }

        // ✅ LẤY CHI TIẾT YÊU CẦU HỦY
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var huy = await _huyDatPhongRepository.GetByIdAsync(id);

            if (huy == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy yêu cầu hủy" });
            }

            return Ok(new { success = true, data = huy });
        }

        // ✅ LẤY DANH SÁCH YÊU CẦU HỦY CỦA KHÁCH HÀNG
        [HttpGet("KhachHang/Me")]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> GetByKhachHang()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var list = await _huyDatPhongRepository.GetByKhachHangAsync(userId);
            return Ok(new { success = true, data = list });
        }

        // ✅ LỄ TÂN DUYỆT/TỪ CHỐI YÊU CẦU HỦY
        [HttpPut("{id}/Duyet")]
        [Authorize(Roles = "Admin,LeTan")]
        public async Task<IActionResult> DuyetHuyDatPhong(int id, [FromBody] DuyetHuyRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int leTanId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var (success, message) = await _huyDatPhongRepository.DuyetHuyDatPhongAsync(
                id,
                request.ChoDuyet,
                leTanId,
                request.GhiChu
            );

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }

        // ✅ THÊM MỚI - ADMIN LẤY DANH SÁCH CHỜ HOÀN TIỀN
        [HttpGet("ChoHoanTien")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDanhSachChoHoanTien()
        {
            var list = await _huyDatPhongRepository.GetDanhSachChoHoanTienAsync();
            return Ok(new { success = true, data = list });
        }

        // ✅ THÊM MỚI - ADMIN XÁC NHẬN ĐÃ HOÀN TIỀN
        [HttpPut("HoanTien/{maHoanTien}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XacNhanHoanTien(int maHoanTien, [FromBody] XacNhanHoanTienRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int adminId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var (success, message) = await _huyDatPhongRepository.XacNhanHoanTienAsync(
                maHoanTien,
                adminId,
                request.GhiChu
            );

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }
    }
}