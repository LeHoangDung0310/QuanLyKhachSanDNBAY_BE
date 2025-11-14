using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.XacThuc;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using DoAnTotNghiep_KS_BE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class DangKyRepository : IDangKyRepository
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DangKyRepository> _logger;
        private readonly IEmailService _emailService;

        public DangKyRepository(
            MyDbContext context,
            IConfiguration configuration,
            ILogger<DangKyRepository> logger,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<DangKyResponseDTO> DangKyAsync(DangKyDTO dangKyDTO)
        {
            try
            {
                var normalizedEmail = dangKyDTO.Email.Trim().ToLower();

                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (existingUser != null)
                {
                    // Nếu tài khoản đã tồn tại nhưng chưa xác thực
                    if (existingUser.TrangThai == "Chưa xác thực")
                    {
                        // Xóa OTP cũ
                        var oldOTPs = await _context.OTPs
                            .Where(o => o.MaNguoiDung == existingUser.MaNguoiDung && !o.DaSuDung)
                            .ToListAsync();

                        foreach (var oldOTP in oldOTPs)
                        {
                            oldOTP.DaSuDung = true;
                        }

                        // Tạo OTP mới
                        var otpCode = GenerateOTP();
                        var otp = new OTP
                        {
                            MaNguoiDung = existingUser.MaNguoiDung,
                            MaXacThuc = otpCode,
                            Loai = "DangKy",
                            ThoiGianTao = DateTime.Now,
                            HetHanSau = DateTime.Now.AddMinutes(5),
                            DaSuDung = false
                        };

                        await _context.OTPs.AddAsync(otp);
                        await _context.SaveChangesAsync();

                        // Gửi email OTP
                        var emailSent = await _emailService.SendOTPEmailAsync(
                            dangKyDTO.Email,
                            otpCode,
                            dangKyDTO.HoTen
                        );

                        if (!emailSent)
                        {
                            return new DangKyResponseDTO
                            {
                                Success = false,
                                Message = "Không thể gửi email xác thực. Vui lòng thử lại!"
                            };
                        }

                        return new DangKyResponseDTO
                        {
                            Success = true,
                            Message = "Mã OTP đã được gửi lại. Vui lòng kiểm tra email!",
                            Email = dangKyDTO.Email
                        };
                    }

                    // Email đã tồn tại và đã kích hoạt
                    _logger.LogWarning($"Registration failed: Email already exists - {normalizedEmail}");
                    return new DangKyResponseDTO
                    {
                        Success = false,
                        Message = "Email đã được sử dụng!"
                    };
                }

                // Tạo user mới với trạng thái chưa xác thực
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dangKyDTO.MatKhau);

                var newUser = new NguoiDung
                {
                    Email = normalizedEmail,
                    MatKhau = hashedPassword,
                    VaiTro = "KhachHang",
                    HoTen = dangKyDTO.HoTen,
                    SoDienThoai = dangKyDTO.SoDienThoai,
                    TrangThai = "Chưa xác thực",
                    NgayTao = DateTime.Now
                };

                await _context.NguoiDungs.AddAsync(newUser);
                await _context.SaveChangesAsync();

                // Tạo OTP cho user mới
                var newOtpCode = GenerateOTP();
                var newOtp = new OTP
                {
                    MaNguoiDung = newUser.MaNguoiDung,
                    MaXacThuc = newOtpCode,
                    Loai = "DangKy",
                    ThoiGianTao = DateTime.Now,
                    HetHanSau = DateTime.Now.AddMinutes(5),
                    DaSuDung = false
                };

                await _context.OTPs.AddAsync(newOtp);
                await _context.SaveChangesAsync();

                // Gửi email OTP
                var newEmailSent = await _emailService.SendOTPEmailAsync(
                    dangKyDTO.Email,
                    newOtpCode,
                    dangKyDTO.HoTen
                );

                if (!newEmailSent)
                {
                    // Xóa user vừa tạo nếu gửi email thất bại
                    _context.NguoiDungs.Remove(newUser);
                    await _context.SaveChangesAsync();

                    return new DangKyResponseDTO
                    {
                        Success = false,
                        Message = "Không thể gửi email xác thực. Vui lòng thử lại!"
                    };
                }

                _logger.LogInformation($"User registered successfully: {normalizedEmail}");

                return new DangKyResponseDTO
                {
                    Success = true,
                    Message = "Đăng ký thành công! Vui lòng kiểm tra email để nhận mã OTP.",
                    Email = dangKyDTO.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Registration error for email: {dangKyDTO.Email}");
                return new DangKyResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại!"
                };
            }
        }

        public async Task<XacThucOTPResponseDTO> XacThucOTPAsync(XacThucOTPDTO xacThucOTPDTO, string? ipAddress)
        {
            try
            {
                var normalizedEmail = xacThucOTPDTO.Email.Trim().ToLower();

                // Tìm user
                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning($"OTP verification failed: User not found - {normalizedEmail}");
                    return new XacThucOTPResponseDTO
                    {
                        Success = false,
                        Message = "Email không tồn tại!"
                    };
                }

                // ✅ KIỂM TRA TRẠNG THÁI TÀI KHOẢN TRƯỚC
                if (user.TrangThai == "Hoạt động")
                {
                    _logger.LogWarning($"OTP verification failed: Account already active - {user.Email}");
                    return new XacThucOTPResponseDTO
                    {
                        Success = false,
                        Message = "Tài khoản đã được kích hoạt! Vui lòng đăng nhập."
                    };
                }

                // Kiểm tra OTP
                var otp = await _context.OTPs
                    .Where(o => o.MaNguoiDung == user.MaNguoiDung
                        && o.MaXacThuc == xacThucOTPDTO.MaOTP
                        && o.Loai == "DangKy"  // ✅ ĐẢM BẢO LOẠI LÀ "DangKy"
                        && !o.DaSuDung
                        && o.HetHanSau > DateTime.Now)
                    .OrderByDescending(o => o.ThoiGianTao)
                    .FirstOrDefaultAsync();

                if (otp == null)
                {
                    _logger.LogWarning($"OTP verification failed: Invalid or expired OTP - {user.Email}");
                    return new XacThucOTPResponseDTO
                    {
                        Success = false,
                        Message = "Mã OTP không hợp lệ hoặc đã hết hạn!"
                    };
                }

                // ✅ SỬ DỤNG TRANSACTION ĐỂ ĐẢM BẢO TÍNH NHẤT QUÁN
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Đánh dấu OTP đã sử dụng
                    otp.DaSuDung = true;

                    // Kích hoạt tài khoản
                    user.TrangThai = "Hoạt động";

                    await _context.SaveChangesAsync();

                    // Generate tokens để tự động đăng nhập
                    var accessToken = GenerateAccessToken(user);
                    var refreshToken = GenerateRefreshToken();

                    // Lưu refresh token
                    var refreshTokenEntity = new RefreshToken
                    {
                        MaNguoiDung = user.MaNguoiDung,
                        Token = refreshToken,
                        NgayTao = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddDays(7),
                        DiaChi = ipAddress ?? "Unknown",
                        DaSuDung = false
                    };

                    await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                    await _context.SaveChangesAsync();

                    // ✅ COMMIT TRANSACTION
                    await transaction.CommitAsync();

                    _logger.LogInformation($"OTP verified successfully for user: {user.Email}");

                    return new XacThucOTPResponseDTO
                    {
                        Success = true,
                        Message = "Xác thực thành công! Tài khoản đã được kích hoạt.",
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        UserInfo = new UserInfoDTO
                        {
                            MaNguoiDung = user.MaNguoiDung,
                            Email = user.Email,
                            VaiTro = user.VaiTro,
                            HoTen = user.HoTen,
                            SoDienThoai = user.SoDienThoai,
                            AnhDaiDien = user.AnhDaiDien
                        }
                    };
                }
                catch (Exception innerEx)
                {
                    // ✅ ROLLBACK NẾU CÓ LỖI
                    await transaction.RollbackAsync();
                    _logger.LogError(innerEx, $"Transaction error during OTP verification for user: {user.Email}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OTP verification error for email: {xacThucOTPDTO.Email}");
                return new XacThucOTPResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình xác thực. Vui lòng thử lại!"
                };
            }
        }

        public async Task<DangKyResponseDTO> GuiLaiOTPAsync(GuiLaiOTPDTO guiLaiOTPDTO)
        {
            try
            {
                var normalizedEmail = guiLaiOTPDTO.Email.Trim().ToLower();

                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning($"Resend OTP failed: User not found - {normalizedEmail}");
                    return new DangKyResponseDTO
                    {
                        Success = false,
                        Message = "Email không tồn tại!"
                    };
                }

                // ✅ KIỂM TRA TRẠNG THÁI TÀI KHOẢN
                if (user.TrangThai == "Hoạt động")
                {
                    _logger.LogWarning($"Resend OTP failed: Account already active - {user.Email}");
                    return new DangKyResponseDTO
                    {
                        Success = false,
                        Message = "Tài khoản đã được kích hoạt! Vui lòng đăng nhập."
                    };
                }

                // Vô hiệu hóa các OTP cũ của loại "DangKy"
                var oldOTPs = await _context.OTPs
                    .Where(o => o.MaNguoiDung == user.MaNguoiDung
                        && o.Loai == "DangKy"  // ✅ CHỈ VÔ HIỆU HÓA OTP ĐĂNG KÝ
                        && !o.DaSuDung)
                    .ToListAsync();

                foreach (var oldOTP in oldOTPs)
                {
                    oldOTP.DaSuDung = true;
                }

                // Tạo OTP mới
                var otpCode = GenerateOTP();
                var otp = new OTP
                {
                    MaNguoiDung = user.MaNguoiDung,
                    MaXacThuc = otpCode,
                    Loai = "DangKy",  // ✅ ĐẢM BẢO LOẠI LÀ "DangKy"
                    ThoiGianTao = DateTime.Now,
                    HetHanSau = DateTime.Now.AddMinutes(5),
                    DaSuDung = false
                };

                await _context.OTPs.AddAsync(otp);
                await _context.SaveChangesAsync();

                // Gửi email OTP
                var emailSent = await _emailService.SendOTPEmailAsync(
                    guiLaiOTPDTO.Email,
                    otpCode,
                    user.HoTen ?? "KhachHang"
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Resend OTP failed: Email sending failed - {user.Email}");
                    return new DangKyResponseDTO
                    {
                        Success = false,
                        Message = "Không thể gửi email. Vui lòng thử lại!"
                    };
                }

                _logger.LogInformation($"OTP resent successfully to: {user.Email}");

                return new DangKyResponseDTO
                {
                    Success = true,
                    Message = "Mã OTP mới đã được gửi. Vui lòng kiểm tra email!",
                    Email = guiLaiOTPDTO.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Resend OTP error for email: {guiLaiOTPDTO.Email}");
                return new DangKyResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi. Vui lòng thử lại!"
                };
            }
        }

        // Helper methods
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateAccessToken(NguoiDung nguoiDung)
        {
            var secretKey = _configuration["AppSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Email, nguoiDung.Email),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro),
                new Claim(ClaimTypes.Name, nguoiDung.HoTen ?? ""),
                new Claim("MaNguoiDung", nguoiDung.MaNguoiDung.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "DoAnTotNghiep_KS",
                audience: "DoAnTotNghiep_KS_Client",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}