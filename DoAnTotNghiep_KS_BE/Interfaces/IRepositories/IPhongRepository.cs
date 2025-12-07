using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.Phong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IPhongRepository
    {
        Task<IEnumerable<PhongDTO>> GetAllPhongsAsync();
        Task<PhongDTO?> GetPhongByIdAsync(int maPhong);
        Task<(IEnumerable<PhongDTO> data, int total)> SearchPhongsAsync(SearchPhongDTO searchDTO);
        Task<Phong> CreatePhongAsync(CreatePhongDTO createPhongDTO);
        Task<bool> UpdatePhongAsync(int maPhong, UpdatePhongDTO updatePhongDTO);
        Task<bool> DeletePhongAsync(int maPhong);
        Task<bool> PhongExistsAsync(int maPhong);
        Task<bool> SoPhongExistsAsync(string soPhong);

        // ✅ THÊM METHOD MỚI
        Task<IEnumerable<PhongTrongDTO>> GetPhongTrongAsync(DateTime ngayNhanPhong, DateTime ngayTraPhong);
    }
}