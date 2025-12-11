using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.XacThuc;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;


namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginRepository> _logger;

        public LoginRepository(MyDbContext context, IConfiguration configuration, ILogger<LoginRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO, string ipAddress)
        {
            try
            {
                var normalizedEmail = loginDTO.Email.Trim().ToLower();

                _logger.LogInformation($"[Login] Attempting login for: {normalizedEmail}");

                var user = await _context.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning($"[Login] User not found: {normalizedEmail}");
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Email hoặc mật khẩu không đúng!"
                    };
                }

                _logger.LogInformation($"[Login] User found: MaNguoiDung={user.MaNguoiDung}, TrangThai={user.TrangThai}");

                // ✅ Kiểm tra mật khẩu có tồn tại không
                if (string.IsNullOrEmpty(user.MatKhau))
                {
                    _logger.LogWarning($"[Login] User {normalizedEmail} has no password (Google login)");
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Tài khoản đăng nhập bằng Google. Vui lòng sử dụng đăng nhập Google!"
                    };
                }

                // ✅ Kiểm tra mật khẩu: BCrypt hash hoặc plain text
                bool isPasswordValid = false;
                bool isPlainTextPassword = false;

                // Kiểm tra xem có phải BCrypt hash không
                if (user.MatKhau.StartsWith("$2a$") || user.MatKhau.StartsWith("$2b$") || user.MatKhau.StartsWith("$2y$"))
                {
                    // Mật khẩu đã được hash bằng BCrypt
                    try
                    {
                        isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.MatKhau, user.MatKhau);
                        _logger.LogInformation($"[Login] BCrypt verification result: {isPasswordValid}");
                    }
                    catch (Exception bcryptEx)
                    {
                        _logger.LogError(bcryptEx, $"[Login] BCrypt verification error");
                        return new LoginResponseDTO
                        {
                            Success = false,
                            Message = "Đã xảy ra lỗi khi xác thực mật khẩu!"
                        };
                    }
                }
                else
                {
                    // ⚠️ Mật khẩu chưa được hash (plain text) - SO SÁNH TRỰC TIẾP
                    _logger.LogWarning($"[Login] Plain text password detected for user: {normalizedEmail}");
                    isPasswordValid = user.MatKhau == loginDTO.MatKhau;
                    isPlainTextPassword = true;
                    _logger.LogInformation($"[Login] Plain text verification result: {isPasswordValid}");
                }

                if (!isPasswordValid)
                {
                    _logger.LogWarning($"[Login] Invalid password for user: {normalizedEmail}");
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Email hoặc mật khẩu không đúng!"
                    };
                }

                // ✅ Nếu đăng nhập thành công với plain text password -> TỰ ĐỘNG HASH VÀ CẬP NHẬT
                if (isPlainTextPassword)
                {
                    try
                    {
                        _logger.LogInformation($"[Login] Converting plain text password to BCrypt hash for user: {normalizedEmail}");
                        user.MatKhau = BCrypt.Net.BCrypt.HashPassword(loginDTO.MatKhau);
                        _context.NguoiDungs.Update(user);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"[Login] Password successfully hashed for user: {normalizedEmail}");
                    }
                    catch (Exception hashEx)
                    {
                        _logger.LogError(hashEx, $"[Login] Error hashing password for user: {normalizedEmail}");
                        // Không return error, vẫn cho đăng nhập thành công
                    }
                }

                // Kiểm tra trạng thái tài khoản
                if (user.TrangThai != "Hoạt động")
                {
                    _logger.LogWarning($"[Login] Account not active: {user.TrangThai}");
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = user.TrangThai == "Chưa xác thực"
                            ? "Tài khoản chưa được xác thực. Vui lòng kiểm tra email!"
                            : "Tài khoản không hoạt động. Vui lòng liên hệ quản trị viên!"
                    };
                }

                _logger.LogInformation($"[Login] Login successful for user: {user.Email}");

                // Tạo tokens
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();

                // ✅ LƯU REFRESH TOKEN VỚI THỜI HẠN 30 NGÀY
                try
                {
                    var refreshTokenEntity = new RefreshToken
                    {
                        MaNguoiDung = user.MaNguoiDung,
                        Token = refreshToken,
                        NgayTao = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddDays(30), // ← 30 ngày (thay vì 7 ngày)
                        DiaChi = ipAddress ?? "Unknown",
                        DaSuDung = false
                    };

                    _logger.LogInformation($"[Login] Attempting to save refresh token for user: {user.MaNguoiDung}");

                    await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"[Login] Refresh token saved successfully");
                }
                catch (Exception tokenEx)
                {
                    _logger.LogError(tokenEx, $"[Login] Error saving refresh token");
                    _logger.LogError($"[Login] Token Error Message: {tokenEx.Message}");
                    _logger.LogError($"[Login] Token Error StackTrace: {tokenEx.StackTrace}");

                    if (tokenEx.InnerException != null)
                    {
                        _logger.LogError($"[Login] Token Inner Exception: {tokenEx.InnerException.Message}");
                    }

                    // ✅ VẪN TRẢ VỀ TOKEN CHO USER (không lưu refresh token)
                    return new LoginResponseDTO
                    {
                        Success = true,
                        Message = "Đăng nhập thành công!",
                        AccessToken = accessToken,
                        RefreshToken = null, // ← Không có refresh token
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

                return new LoginResponseDTO
                {
                    Success = true,
                    Message = "Đăng nhập thành công!",
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Login] Unexpected error for email: {loginDTO.Email}");
                _logger.LogError($"[Login] Message: {ex.Message}");
                _logger.LogError($"[Login] StackTrace: {ex.StackTrace}");

                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại!"
                };
            }
        }

        public async Task<RefreshTokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO, string? ipAddress)
        {
            try
            {
                var refreshTokenEntity = await _context.RefreshTokens
                    .Include(rt => rt.NguoiDung)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDTO.RefreshToken
                        && !rt.DaSuDung
                        && rt.NgayHetHan > DateTime.Now);

                if (refreshTokenEntity == null || refreshTokenEntity.NguoiDung == null)
                {
                    return new RefreshTokenResponseDTO
                    {
                        Success = false,
                        Message = "Refresh token không hợp lệ hoặc đã hết hạn!"
                    };
                }

                // Kiểm tra trạng thái người dùng
                if (refreshTokenEntity.NguoiDung.TrangThai != "Hoạt động")
                {
                    return new RefreshTokenResponseDTO
                    {
                        Success = false,
                        Message = "Tài khoản đã bị khóa hoặc không hoạt động!"
                    };
                }

                // Revoke old token
                refreshTokenEntity.DaSuDung = true;
                refreshTokenEntity.NgaySuDung = DateTime.Now;

                // ✅ TẠO TOKEN MỚI VỚI THỜI GIAN DÀI HƠN
                var newAccessToken = GenerateAccessToken(refreshTokenEntity.NguoiDung); // 24 giờ
                var newRefreshToken = GenerateRefreshToken();

                // ✅ LƯU REFRESH TOKEN MỚI VỚI THỜI HẠN 30 NGÀY
                var newRefreshTokenEntity = new RefreshToken
                {
                    MaNguoiDung = refreshTokenEntity.MaNguoiDung,
                    Token = newRefreshToken,
                    NgayTao = DateTime.Now,
                    NgayHetHan = DateTime.Now.AddDays(30), // ← 30 ngày
                    DiaChi = ipAddress
                };

                await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                return new RefreshTokenResponseDTO
                {
                    Success = true,
                    Message = "Làm mới token thành công!",
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                return new RefreshTokenResponseDTO
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .Include(rt => rt.NguoiDung)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.DaSuDung);

                if (token != null)
                {
                    token.DaSuDung = true;
                    token.NgaySuDung = DateTime.Now;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"User logged out: {token.NguoiDung?.Email} - IP: {token.DiaChi}");
                    return true;
                }

                _logger.LogWarning($"Logout failed: Token not found or already used");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> LogoutAllAsync(int maNguoiDung)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.MaNguoiDung == maNguoiDung && !rt.DaSuDung)
                    .ToListAsync();

                if (tokens.Any())
                {
                    foreach (var token in tokens)
                    {
                        token.DaSuDung = true;
                        token.NgaySuDung = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"User logged out from all devices: MaNguoiDung={maNguoiDung}, Devices={tokens.Count}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during logout all for user {maNguoiDung}");
                return false;
            }
        }

        public async Task<bool> DoiMatKhauAsync(int maNguoiDung, DoiMatKhauDTO doiMatKhauDTO)
        {
            try
            {
                // Tìm người dùng
                var nguoiDung = await _context.NguoiDungs
                    .FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);

                if (nguoiDung == null)
                {
                    _logger.LogWarning($"Không tìm thấy người dùng với MaNguoiDung: {maNguoiDung}");
                    return false;
                }

                // Kiểm tra tài khoản có mật khẩu không (không phải đăng nhập bằng Google)
                if (string.IsNullOrEmpty(nguoiDung.MatKhau))
                {
                    _logger.LogWarning($"Tài khoản {nguoiDung.Email} đăng nhập bằng Google, không thể đổi mật khẩu");
                    throw new InvalidOperationException("Tài khoản đăng nhập bằng Google không thể đổi mật khẩu");
                }

                // Kiểm tra mật khẩu hiện tại
                bool kiemTraMatKhauHienTai = BCrypt.Net.BCrypt.Verify(
                    doiMatKhauDTO.MatKhauHienTai,
                    nguoiDung.MatKhau
                );

                if (!kiemTraMatKhauHienTai)
                {
                    _logger.LogWarning($"Mật khẩu hiện tại không đúng cho người dùng: {nguoiDung.Email}");
                    return false;
                }

                // Kiểm tra mật khẩu mới không trùng với mật khẩu cũ
                bool trungMatKhauCu = BCrypt.Net.BCrypt.Verify(
                    doiMatKhauDTO.MatKhauMoi,
                    nguoiDung.MatKhau
                );

                if (trungMatKhauCu)
                {
                    _logger.LogWarning($"Mật khẩu mới trùng với mật khẩu cũ cho người dùng: {nguoiDung.Email}");
                    throw new InvalidOperationException("Mật khẩu mới không được trùng với mật khẩu cũ");
                }

                // Mã hóa và cập nhật mật khẩu mới
                nguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(doiMatKhauDTO.MatKhauMoi);

                _context.NguoiDungs.Update(nguoiDung);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Đổi mật khẩu thành công cho người dùng: {nguoiDung.Email}");

                // Đăng xuất tất cả thiết bị sau khi đổi mật khẩu
                await LogoutAllAsync(maNguoiDung);

                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đổi mật khẩu cho người dùng MaNguoiDung: {maNguoiDung}");
                throw;
            }
        }

        // Helper methods
        // ========== TẠO ACCESS TOKEN (TĂNG THỜI GIAN LÊN 24 GIỜ) ==========
        private string GenerateAccessToken(NguoiDung nguoiDung)
        {
            var secretKey = _configuration["AppSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key chưa dc cấu hình");
            }

            // Dùng key để mã hóa
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Tạo credentials để ký token dựa trên key và thuật toán HmacSha256
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Thông tin claim user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Email, nguoiDung.Email),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro),
                new Claim(ClaimTypes.Name, nguoiDung.HoTen ?? ""),
                new Claim("MaNguoiDung", nguoiDung.MaNguoiDung.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
            };

            // ✅ TẠO TOKEN VỚI THỜI GIAN 24 GIỜ (thay vì 30 phút)
            var token = new JwtSecurityToken(
                issuer: "DoAnTotNghiep_KS",
                audience: "DoAnTotNghiep_KS_Client",
                claims: claims,
                expires: DateTime.Now.AddHours(24), // ← 24 giờ
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ========== TẠO REFRESH TOKEN (TĂNG LÊN 30 NGÀY) ==========
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}