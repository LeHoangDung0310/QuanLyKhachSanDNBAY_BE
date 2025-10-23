using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Interfaces.dto;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuenMatKhauController : ControllerBase
    {
        private readonly IQuenMatKhauRepository _quenMatKhauRepository;
        private readonly MyDbContext _context; // ✅ Thêm dòng này

        public QuenMatKhauController(IQuenMatKhauRepository quenMatKhauRepository, MyDbContext context)
        {
            _quenMatKhauRepository = quenMatKhauRepository;
            _context = context; // ✅ Thêm dòng này
        }

        /// <summary>
        /// Gửi OTP để đặt lại mật khẩu
        /// </summary>
        /// <param name="quenMatKhauDTO">Email cần đặt lại mật khẩu</param>
        /// <returns>Kết quả gửi OTP</returns>
        [HttpPost("gui-otp")]
        [ProducesResponseType(typeof(QuenMatKhauResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<QuenMatKhauResponseDTO>> GuiOTP([FromBody] QuenMatKhauDTO quenMatKhauDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new QuenMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Email không hợp lệ!"
                });
            }

            var result = await _quenMatKhauRepository.GuiOTPQuenMatKhauAsync(quenMatKhauDTO);
            return Ok(result);
        }

        /// <summary>
        /// Xác thực OTP trước khi đặt lại mật khẩu
        /// </summary>
        /// <param name="xacThucOTPDTO">Email và mã OTP</param>
        /// <returns>Kết quả xác thực</returns>
        [HttpPost("xac-thuc-otp")]
        [ProducesResponseType(typeof(XacThucOTPQuenMatKhauResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<XacThucOTPQuenMatKhauResponseDTO>> XacThucOTP([FromBody] XacThucOTPQuenMatKhauDTO xacThucOTPDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new XacThucOTPQuenMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!",
                    OTPValid = false
                });
            }

            var result = await _quenMatKhauRepository.XacThucOTPQuenMatKhauAsync(xacThucOTPDTO);
            return Ok(result);
        }

        /// <summary>
        /// Đặt lại mật khẩu mới
        /// </summary>
        /// <param name="datLaiMatKhauDTO">Email, OTP và mật khẩu mới</param>
        /// <returns>Kết quả đặt lại mật khẩu</returns>
        [HttpPost("dat-lai-mat-khau")]
        [ProducesResponseType(typeof(DatLaiMatKhauResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DatLaiMatKhauResponseDTO>> DatLaiMatKhau([FromBody] DatLaiMatKhauDTO datLaiMatKhauDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new DatLaiMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!"
                });
            }

            var result = await _quenMatKhauRepository.DatLaiMatKhauAsync(datLaiMatKhauDTO);
            return Ok(result);
        }

        /// <summary>
        /// [DEBUG] Kiểm tra OTP trong database
        /// </summary>
        [HttpGet("debug-otp/{email}")]
        public async Task<ActionResult> DebugOTP(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null)
                return Ok(new { message = "User not found" });

            var otps = await _context.OTPs
                .Where(o => o.MaNguoiDung == user.MaNguoiDung && o.Loai == "QuenMatKhau")
                .OrderByDescending(o => o.ThoiGianTao)
                .Select(o => new
                {
                    o.MaOTP,
                    o.MaXacThuc,
                    o.DaSuDung,
                    o.ThoiGianTao,
                    o.HetHanSau,
                    IsExpired = o.HetHanSau < DateTime.Now,
                    TimeLeftMinutes = o.HetHanSau > DateTime.Now
                        ? (o.HetHanSau - DateTime.Now).GetValueOrDefault().TotalMinutes
                        : 0
                })
                .ToListAsync();

            return Ok(new
            {
                email = user.Email,
                maNguoiDung = user.MaNguoiDung,
                currentTime = DateTime.Now,
                otps = otps
            });
        }
    }
}