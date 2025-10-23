using DoAnTotNghiep_KS_BE.Interfaces.dto;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangKyController : ControllerBase
    {
        private readonly IDangKyRepository _dangKyRepository;

        public DangKyController(IDangKyRepository dangKyRepository)
        {
            _dangKyRepository = dangKyRepository;
        }

        /// <summary>
        /// Đăng ký tài khoản mới - Gửi OTP qua email
        /// </summary>
        /// <param name="dangKyDTO">Thông tin đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        [HttpPost("dang-ky")]
        [ProducesResponseType(typeof(DangKyResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DangKyResponseDTO>> DangKy([FromBody] DangKyDTO dangKyDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new DangKyResponseDTO
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!"
                });
            }

            var result = await _dangKyRepository.DangKyAsync(dangKyDTO);
            return Ok(result);
        }

        /// <summary>
        /// Xác thực OTP để kích hoạt tài khoản
        /// </summary>
        /// <param name="xacThucOTPDTO">Email và mã OTP</param>
        /// <returns>Token và thông tin người dùng nếu xác thực thành công</returns>
        [HttpPost("xac-thuc-otp")]
        [ProducesResponseType(typeof(XacThucOTPResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<XacThucOTPResponseDTO>> XacThucOTP([FromBody] XacThucOTPDTO xacThucOTPDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new XacThucOTPResponseDTO
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!"
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _dangKyRepository.XacThucOTPAsync(xacThucOTPDTO, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Gửi lại mã OTP
        /// </summary>
        /// <param name="guiLaiOTPDTO">Email cần gửi lại OTP</param>
        /// <returns>Kết quả gửi OTP</returns>
        [HttpPost("gui-lai-otp")]
        [ProducesResponseType(typeof(DangKyResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<DangKyResponseDTO>> GuiLaiOTP([FromBody] GuiLaiOTPDTO guiLaiOTPDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new DangKyResponseDTO
                {
                    Success = false,
                    Message = "Email không hợp lệ!"
                });
            }

            var result = await _dangKyRepository.GuiLaiOTPAsync(guiLaiOTPDTO);
            return Ok(result);
        }
    }
}