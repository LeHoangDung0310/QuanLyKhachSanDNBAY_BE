using DoAnTotNghiep_KS_BE.Interfaces.dto;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace DoAnTotNghiep_KS_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginRepository loginRepository, ILogger<LoginController> logger)
        {
            _loginRepository = loginRepository;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập với email và mật khẩu
        /// </summary>
        /// <param name="loginDTO">Thông tin đăng nhập</param>
        /// <returns>Token và thông tin người dùng</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new LoginResponseDTO
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ!"
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _loginRepository.LoginAsync(loginDTO, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Làm mới access token bằng refresh token
        /// </summary>
        /// <param name="refreshTokenDTO">Refresh token</param>
        /// <returns>Token mới</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(RefreshTokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<RefreshTokenResponseDTO>> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            if (string.IsNullOrEmpty(refreshTokenDTO.RefreshToken))
            {
                return Ok(new RefreshTokenResponseDTO
                {
                    Success = false,
                    Message = "Refresh token không được để trống!"
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _loginRepository.RefreshTokenAsync(refreshTokenDTO, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Đăng xuất khỏi thiết bị hiện tại
        /// </summary>
        /// <param name="refreshTokenDTO">Refresh token cần thu hồi</param>
        /// <returns>Kết quả đăng xuất</returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            if (string.IsNullOrEmpty(refreshTokenDTO?.RefreshToken))
            {
                return BadRequest(new { success = false, message = "Refresh token không được để trống!" });
            }

            var result = await _loginRepository.LogoutAsync(refreshTokenDTO.RefreshToken);

            if (result)
            {
                return Ok(new { success = true, message = "Đăng xuất thành công!" });
            }

            return BadRequest(new { success = false, message = "Đăng xuất thất bại!" });
        }

        /// <summary>
        /// Đăng xuất khỏi tất cả thiết bị
        /// </summary>
        /// <returns>Kết quả đăng xuất</returns>
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var maNguoiDungClaim = User.FindFirst("MaNguoiDung")?.Value;

            if (string.IsNullOrEmpty(maNguoiDungClaim) || !int.TryParse(maNguoiDungClaim, out int maNguoiDung))
            {
                return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng!" });
            }

            var result = await _loginRepository.LogoutAllAsync(maNguoiDung);

            if (result)
            {
                return Ok(new { success = true, message = "Đăng xuất khỏi tất cả thiết bị thành công!" });
            }

            return BadRequest(new { success = false, message = "Đăng xuất thất bại!" });
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện tại từ token
        /// </summary>
        /// <returns>Thông tin người dùng</returns>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
        public ActionResult<UserInfoDTO> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Success = false, Message = "Token không hợp lệ!" });
            }

            return Ok(new UserInfoDTO
            {
                MaNguoiDung = int.Parse(userId),
                Email = email ?? "",
                VaiTro = role ?? "",
                HoTen = name
            });
        }

        /// <summary>
        /// Đổi mật khẩu người dùng
        /// </summary>
        /// <param name="doiMatKhauDTO">Thông tin đổi mật khẩu</param>
        /// <returns>Kết quả đổi mật khẩu</returns>
        [HttpPost("doi-mat-khau")]
        [Authorize]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDTO doiMatKhauDTO)
        {
            try
            {
                // Lấy MaNguoiDung từ token
                var maNguoiDungClaim = User.FindFirst("MaNguoiDung")?.Value;

                if (string.IsNullOrEmpty(maNguoiDungClaim) || !int.TryParse(maNguoiDungClaim, out int maNguoiDung))
                {
                    return Unauthorized(new
                    {
                        thanhCong = false,
                        thongBao = "Không tìm thấy thông tin người dùng!"
                    });
                }

                var ketQua = await _loginRepository.DoiMatKhauAsync(maNguoiDung, doiMatKhauDTO);

                if (ketQua)
                {
                    return Ok(new
                    {
                        thanhCong = true,
                        thongBao = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại."
                    });
                }

                return BadRequest(new
                {
                    thanhCong = false,
                    thongBao = "Mật khẩu hiện tại không đúng!"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    thanhCong = false,
                    thongBao = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu");
                return StatusCode(500, new
                {
                    thanhCong = false,
                    thongBao = "Đã xảy ra lỗi khi đổi mật khẩu!"
                });
            }
        }
    }
}