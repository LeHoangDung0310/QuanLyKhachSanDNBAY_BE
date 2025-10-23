using DoAnTotNghiep_KS_BE.Interfaces.dto;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface ILoginRepository
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO, string? ipAddress);
        Task<RefreshTokenResponseDTO> RefreshTokenAsync(RefreshTokenDTO refreshTokenDTO, string? ipAddress);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> LogoutAllAsync(int maNguoiDung);
        Task<bool> DoiMatKhauAsync(int maNguoiDung, DoiMatKhauDTO doiMatKhauDTO);
    }
}