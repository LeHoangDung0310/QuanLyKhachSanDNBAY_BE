using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IHinhAnhPhongRepository
    {
        Task<IEnumerable<HinhAnhPhongDTO>> GetAllHinhAnhPhongsAsync();
        Task<IEnumerable<HinhAnhPhongDTO>> GetHinhAnhsByPhongIdAsync(int maPhong);
        Task<HinhAnhPhongDTO?> GetHinhAnhPhongByIdAsync(int maHinhAnh);
        Task<(IEnumerable<HinhAnhPhongDTO> data, int total)> SearchHinhAnhPhongsAsync(SearchHinhAnhPhongDTO searchDTO);
        Task<HinhAnhPhong> CreateHinhAnhPhongAsync(CreateHinhAnhPhongDTO createHinhAnhPhongDTO);
        Task<bool> UpdateHinhAnhPhongAsync(int maHinhAnh, UpdateHinhAnhPhongDTO updateHinhAnhPhongDTO);
        Task<bool> DeleteHinhAnhPhongAsync(int maHinhAnh);
        Task<bool> HinhAnhPhongExistsAsync(int maHinhAnh);
    }
}