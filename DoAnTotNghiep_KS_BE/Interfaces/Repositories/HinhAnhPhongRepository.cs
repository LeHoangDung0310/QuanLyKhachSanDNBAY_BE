using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class HinhAnhPhongRepository : IHinhAnhPhongRepository
    {
        private readonly MyDbContext _context;

        public HinhAnhPhongRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HinhAnhPhongDTO>> GetAllHinhAnhPhongsAsync()
        {
            return await _context.HinhAnhPhongs
                .Include(h => h.Phong)
                .Select(h => new HinhAnhPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaPhong = h.MaPhong,
                    SoPhong = h.Phong != null ? h.Phong.SoPhong : null,
                    Url = h.Url
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<HinhAnhPhongDTO>> GetHinhAnhsByPhongIdAsync(int maPhong)
        {
            return await _context.HinhAnhPhongs
                .Include(h => h.Phong)
                .Where(h => h.MaPhong == maPhong)
                .Select(h => new HinhAnhPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaPhong = h.MaPhong,
                    SoPhong = h.Phong != null ? h.Phong.SoPhong : null,
                    Url = h.Url
                })
                .ToListAsync();
        }

        public async Task<HinhAnhPhongDTO?> GetHinhAnhPhongByIdAsync(int maHinhAnh)
        {
            return await _context.HinhAnhPhongs
                .Include(h => h.Phong)
                .Where(h => h.MaHinhAnh == maHinhAnh)
                .Select(h => new HinhAnhPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaPhong = h.MaPhong,
                    SoPhong = h.Phong != null ? h.Phong.SoPhong : null,
                    Url = h.Url
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<HinhAnhPhongDTO> data, int total)> SearchHinhAnhPhongsAsync(SearchHinhAnhPhongDTO searchDTO)
        {
            var query = _context.HinhAnhPhongs.Include(h => h.Phong).AsQueryable();

            // Lọc theo mã phòng
            if (searchDTO.MaPhong.HasValue)
            {
                query = query.Where(h => h.MaPhong == searchDTO.MaPhong.Value);
            }

            // Tìm kiếm theo số phòng
            if (!string.IsNullOrWhiteSpace(searchDTO.SoPhong))
            {
                var soPhong = searchDTO.SoPhong.ToLower().Trim();
                query = query.Where(h => h.Phong != null && h.Phong.SoPhong != null &&
                                        h.Phong.SoPhong.ToLower().Contains(soPhong));
            }

            // Đếm tổng số bản ghi
            var total = await query.CountAsync();

            // Phân trang
            var data = await query
                .OrderBy(h => h.MaPhong)
                .ThenBy(h => h.MaHinhAnh)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(h => new HinhAnhPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaPhong = h.MaPhong,
                    SoPhong = h.Phong != null ? h.Phong.SoPhong : null,
                    Url = h.Url
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<HinhAnhPhong> CreateHinhAnhPhongAsync(CreateHinhAnhPhongDTO createHinhAnhPhongDTO)
        {
            var hinhAnhPhong = new HinhAnhPhong
            {
                MaPhong = createHinhAnhPhongDTO.MaPhong,
                Url = createHinhAnhPhongDTO.Url
            };

            _context.HinhAnhPhongs.Add(hinhAnhPhong);
            await _context.SaveChangesAsync();
            return hinhAnhPhong;
        }

        public async Task<bool> UpdateHinhAnhPhongAsync(int maHinhAnh, UpdateHinhAnhPhongDTO updateHinhAnhPhongDTO)
        {
            var hinhAnhPhong = await _context.HinhAnhPhongs.FindAsync(maHinhAnh);
            if (hinhAnhPhong == null) return false;

            if (updateHinhAnhPhongDTO.Url != null) hinhAnhPhong.Url = updateHinhAnhPhongDTO.Url;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHinhAnhPhongAsync(int maHinhAnh)
        {
            var hinhAnhPhong = await _context.HinhAnhPhongs.FindAsync(maHinhAnh);
            if (hinhAnhPhong == null) return false;

            _context.HinhAnhPhongs.Remove(hinhAnhPhong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HinhAnhPhongExistsAsync(int maHinhAnh)
        {
            return await _context.HinhAnhPhongs.AnyAsync(h => h.MaHinhAnh == maHinhAnh);
        }
    }
}