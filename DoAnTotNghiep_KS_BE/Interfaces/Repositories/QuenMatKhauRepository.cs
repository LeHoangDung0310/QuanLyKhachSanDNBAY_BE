using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using DoAnTotNghiep_KS_BE.Services;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class QuenMatKhauRepository : IQuenMatKhauRepository
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<QuenMatKhauRepository> _logger;
        private readonly IEmailService _emailService;

        public QuenMatKhauRepository(
            MyDbContext context,
            IConfiguration configuration,
            ILogger<QuenMatKhauRepository> logger,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<QuenMatKhauResponseDTO> GuiOTPQuenMatKhauAsync(QuenMatKhauDTO quenMatKhauDTO)
        {
            try
            {
                var normalizedEmail = quenMatKhauDTO.Email.Trim().ToLower();

                // Tìm user theo email
                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning($"Password reset requested for non-existent email: {normalizedEmail}");
                    return new QuenMatKhauResponseDTO
                    {
                        Success = true,
                        Message = "Nếu email tồn tại trong hệ thống, mã OTP đã được gửi. Vui lòng kiểm tra email!",
                        Email = quenMatKhauDTO.Email
                    };
                }

                // Kiểm tra trạng thái tài khoản
                if (user.TrangThai != "Hoạt động")
                {
                    return new QuenMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Tài khoản không hoạt động. Vui lòng liên hệ quản trị viên!"
                    };
                }

                // Vô hiệu hóa các OTP cũ
                var oldOTPs = await _context.OTPs
                    .Where(o => o.MaNguoiDung == user.MaNguoiDung
                        && o.Loai == "QuenMatKhau"
                        && !o.DaSuDung)
                    .ToListAsync();

                foreach (var oldOtp in oldOTPs) // ✅ Đổi tên biến thành oldOtp (chữ thường)
                {
                    oldOtp.DaSuDung = true;
                }

                // Tạo OTP mới
                var otpCode = GenerateOTP();
                var newOtp = new OTP // ✅ Đổi tên biến thành newOtp
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaXacThuc = otpCode,
                    Loai = "QuenMatKhau",
                    ThoiGianTao = DateTime.Now,
                    HetHanSau = DateTime.Now.AddMinutes(5),
                    DaSuDung = false
                };

                await _context.OTPs.AddAsync(newOtp);
                await _context.SaveChangesAsync();

                // Gửi email OTP
                var emailSent = await _emailService.SendResetPasswordOTPEmailAsync(
                    quenMatKhauDTO.Email,
                    otpCode,
                    user.HoTen ?? "Khách hàng"
                );

                if (!emailSent)
                {
                    return new QuenMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Không thể gửi email. Vui lòng thử lại sau!"
                    };
                }

                _logger.LogInformation($"Password reset OTP sent to: {user.Email}");

                return new QuenMatKhauResponseDTO
                {
                    Success = true,
                    Message = "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra!",
                    Email = quenMatKhauDTO.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending password reset OTP for email: {quenMatKhauDTO.Email}");
                return new QuenMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<XacThucOTPQuenMatKhauResponseDTO> XacThucOTPQuenMatKhauAsync(XacThucOTPQuenMatKhauDTO xacThucOTPDTO)
        {
            try
            {
                var normalizedEmail = xacThucOTPDTO.Email.Trim().ToLower();

                // Tìm user
                var user = await _context.NguoiDungs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    return new XacThucOTPQuenMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Email không tồn tại trong hệ thống!",
                        OTPValid = false
                    };
                }

                // Kiểm tra OTP
                var otp = await _context.OTPs
                    .AsNoTracking()
                    .Where(o => o.MaNguoiDung == user.MaNguoiDung
                        && o.MaXacThuc == xacThucOTPDTO.MaOTP
                        && o.Loai == "QuenMatKhau"
                        && !o.DaSuDung
                        && o.HetHanSau > DateTime.Now)
                    .OrderByDescending(o => o.ThoiGianTao)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    return new XacThucOTPQuenMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Mã OTP không hợp lệ hoặc đã hết hạn!",
                        OTPValid = false
                    };
                }

                _logger.LogInformation($"Password reset OTP verified for user: {user.Email}");

                return new XacThucOTPQuenMatKhauResponseDTO
                {
                    Success = true,
                    Message = "Mã OTP hợp lệ. Vui lòng nhập mật khẩu mới!",
                    OTPValid = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying password reset OTP for email: {xacThucOTPDTO.Email}");
                return new XacThucOTPQuenMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!",
                    OTPValid = false
                };
            }
        }

        public async Task<DatLaiMatKhauResponseDTO> DatLaiMatKhauAsync(DatLaiMatKhauDTO datLaiMatKhauDTO)
        {
            try
            {
                var normalizedEmail = datLaiMatKhauDTO.Email.Trim().ToLower();

                _logger.LogInformation($"[DatLaiMatKhau] Starting - Email: {normalizedEmail}, OTP: {datLaiMatKhauDTO.MaOTP}");

                // Tìm user
                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning($"[DatLaiMatKhau] User not found");
                    return new DatLaiMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Email không tồn tại trong hệ thống!"
                    };
                }

                _logger.LogInformation($"[DatLaiMatKhau] User found: MaNguoiDung={user.MaNguoiDung}");

                // Kiểm tra OTP
                var otp = await _context.OTPs
                    .Where(o => o.MaNguoiDung == user.MaNguoiDung
                        && o.MaXacThuc == datLaiMatKhauDTO.MaOTP
                        && o.Loai == "QuenMatKhau"
                        && !o.DaSuDung
                        && o.HetHanSau > DateTime.Now)
                    .OrderByDescending(o => o.ThoiGianTao)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    _logger.LogWarning($"[DatLaiMatKhau] OTP not found or invalid");

                    var allOTPs = await _context.OTPs
                        .Where(o => o.MaNguoiDung == user.MaNguoiDung && o.Loai == "QuenMatKhau")
                        .OrderByDescending(o => o.ThoiGianTao)
                        .Take(3)
                        .ToListAsync();

                    _logger.LogWarning($"[DatLaiMatKhau] Total QuenMatKhau OTPs: {allOTPs.Count}");
                    foreach (var o in allOTPs)
                    {
                        _logger.LogWarning($"  - OTP: {o.MaXacThuc}, DaSuDung: {o.DaSuDung}, HetHan: {o.HetHanSau:yyyy-MM-dd HH:mm:ss}");
                    }

                    return new DatLaiMatKhauResponseDTO
                    {
                        Success = false,
                        Message = "Mã OTP không hợp lệ hoặc đã hết hạn!"
                    };
                }

                _logger.LogInformation($"[DatLaiMatKhau] Valid OTP found: {otp.MaXacThuc}");

                // Đổi mật khẩu
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(datLaiMatKhauDTO.MatKhauMoi);
                _logger.LogInformation($"[DatLaiMatKhau] Password hashed successfully");

                // Đánh dấu OTP đã sử dụng
                otp.DaSuDung = true;
                _logger.LogInformation($"[DatLaiMatKhau] OTP marked as used");

                // ✅ VÔ HIỆU HÓA TẤT CẢ REFRESH TOKEN - ĐÁ USER RA KHỎI TẤT CẢ THIẾT BỊ
                try
                {
                    var refreshTokens = await _context.RefreshTokens
                        .Where(rt => rt.MaNguoiDung == user.MaNguoiDung && !rt.DaSuDung)
                        .ToListAsync();

                    _logger.LogInformation($"[DatLaiMatKhau] Found {refreshTokens.Count} active refresh tokens to invalidate");

                    if (refreshTokens.Any())
                    {
                        foreach (var token in refreshTokens)
                        {
                            token.DaSuDung = true;
                            token.NgaySuDung = DateTime.Now;
                        }
                        _logger.LogInformation($"[DatLaiMatKhau] Invalidated {refreshTokens.Count} refresh tokens");
                    }
                }
                catch (Exception tokenEx)
                {
                    // ✅ NẾU LỖI VỚI REFRESH TOKEN, VẪN CHO ĐỔI MẬT KHẨU
                    _logger.LogWarning(tokenEx, $"[DatLaiMatKhau] Warning: Could not invalidate refresh tokens, but password will still be changed");
                }

                // Lưu thay đổi
                var savedCount = await _context.SaveChangesAsync();
                _logger.LogInformation($"[DatLaiMatKhau] Saved {savedCount} changes to database");

                return new DatLaiMatKhauResponseDTO
                {
                    Success = true,
                    Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[DatLaiMatKhau] EXCEPTION");
                _logger.LogError($"[DatLaiMatKhau] Message: {ex.Message}");
                _logger.LogError($"[DatLaiMatKhau] StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"[DatLaiMatKhau] InnerException: {ex.InnerException.Message}");
                }

                return new DatLaiMatKhauResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!"
                };
            }
        }

        // Helper method
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}