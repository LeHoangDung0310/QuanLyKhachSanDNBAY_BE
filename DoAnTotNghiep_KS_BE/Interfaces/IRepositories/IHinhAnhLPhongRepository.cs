using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IHinhAnhLPhongRepository
    {
        Task<IEnumerable<HinhAnhLPhongDTO>> GetAllHinhAnhLPhongsAsync();
        Task<HinhAnhLPhongDTO?> GetHinhAnhLPhongByIdAsync(int id);
        Task<IEnumerable<HinhAnhLPhongDTO>> GetHinhAnhsByLoaiPhongIdAsync(int maLoaiPhong);
        Task<(IEnumerable<HinhAnhLPhongDTO> data, int total)> SearchHinhAnhLPhongsAsync(SearchHinhAnhLPhongDTO searchDTO);
        Task<HinhAnhLPhong> CreateHinhAnhLPhongAsync(CreateHinhAnhLPhongDTO createHinhAnhLPhongDTO);
        Task<bool> UpdateHinhAnhLPhongAsync(int id, UpdateHinhAnhLPhongDTO updateHinhAnhLPhongDTO);
        Task<bool> DeleteHinhAnhLPhongAsync(int id);
        Task<bool> HinhAnhLPhongExistsAsync(int id);
    }
}