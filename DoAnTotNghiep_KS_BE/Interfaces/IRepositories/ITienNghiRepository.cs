using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface ITienNghiRepository
    {
        Task<IEnumerable<TienNghiDTO>> GetAllTienNghisAsync();
        Task<TienNghiDTO?> GetTienNghiByIdAsync(int maTienNghi);
        Task<(IEnumerable<TienNghiDTO> data, int total)> SearchTienNghisAsync(SearchTienNghiDTO searchDTO);
        Task<TienNghi> CreateTienNghiAsync(CreateTienNghiDTO createTienNghiDTO);
        Task<bool> UpdateTienNghiAsync(int maTienNghi, UpdateTienNghiDTO updateTienNghiDTO);
        Task<bool> DeleteTienNghiAsync(int maTienNghi);
        Task<bool> TienNghiExistsAsync(int maTienNghi);
    }
}