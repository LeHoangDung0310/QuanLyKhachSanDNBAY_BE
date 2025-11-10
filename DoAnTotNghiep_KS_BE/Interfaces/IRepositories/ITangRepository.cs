using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.Tang;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface ITangRepository
    {
        Task<IEnumerable<TangDTO>> GetAllTangsAsync();
        Task<TangDTO?> GetTangByIdAsync(int maTang);
        Task<(IEnumerable<TangDTO> data, int total)> SearchTangsAsync(SearchTangDTO searchDTO);
        Task<Tang> CreateTangAsync(CreateTangDTO createTangDTO);
        Task<bool> UpdateTangAsync(int maTang, UpdateTangDTO updateTangDTO);
        Task<bool> DeleteTangAsync(int maTang);
        Task<bool> TangExistsAsync(int maTang);
    }
}