using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface ILoaiPhongRepository
    {
        Task<IEnumerable<LoaiPhongDTO>> GetAllLoaiPhongsAsync();
        Task<LoaiPhongDTO?> GetLoaiPhongByIdAsync(int maLoaiPhong);
        Task<(IEnumerable<LoaiPhongDTO> data, int total)> SearchLoaiPhongsAsync(SearchLoaiPhongDTO searchDTO);
        Task<LoaiPhong> CreateLoaiPhongAsync(CreateLoaiPhongDTO createLoaiPhongDTO);
        Task<bool> UpdateLoaiPhongAsync(int id, UpdateLoaiPhongDTO updateLoaiPhongDTO);
        Task<bool> DeleteLoaiPhongAsync(int id);
        Task<bool> LoaiPhongExistsAsync(int id);
    }
}