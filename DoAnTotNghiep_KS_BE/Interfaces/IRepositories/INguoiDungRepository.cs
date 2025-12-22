using DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface INguoiDungRepository
    {
        Task<IEnumerable<NguoiDungDTO>> GetAllNguoiDungsAsync();
        Task<IEnumerable<NguoiDungDTO>> GetNguoiDungsByRoleAsync(string vaiTro);
        Task<NguoiDungDTO?> GetNguoiDungByIdAsync(int maNguoiDung);
        Task<(IEnumerable<NguoiDungDTO> data, int total)> SearchNguoiDungsAsync(SearchNguoiDungDTO searchDTO);
        Task<bool> UpdateNguoiDungAsync(int maNguoiDung, UpdateNguoiDungAdminDTO updateDTO);
        Task<bool> DeleteNguoiDungAsync(int maNguoiDung);
        Task<bool> NguoiDungExistsAsync(int maNguoiDung);
        Task<bool> CanDeleteNguoiDungAsync(int maNguoiDung);

        // Profile methods
        Task<bool> UpdateProfileAsync(int maNguoiDung, UpdateProfileDTO updateDTO);
        Task<(bool Success, string Message)> ChangePasswordAsync(int maNguoiDung, ChangePasswordDTO changePasswordDTO);
        Task<bool> UpdateAvatarAsync(int maNguoiDung, string avatarUrl);

        // Lấy thông tin người dùng theo email (auto-fill khi đặt phòng trực tiếp)
        Task<NguoiDungDTO?> GetNguoiDungByEmailAsync(string email);
    }
}