using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.Tang;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class TangRepository : ITangRepository
    {
        private readonly MyDbContext _context;

        public TangRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TangDTO>> GetAllTangsAsync()
        {
            return await _context.Tangs
                .Include(t => t.Phongs)
                .Select(t => new TangDTO
                {
                    MaTang = t.MaTang,
                    TenTang = t.TenTang,
                    MoTa = t.MoTa,
                    SoPhong = t.Phongs != null ? t.Phongs.Count : 0
                })
                .ToListAsync();
        }

        public async Task<TangDTO?> GetTangByIdAsync(int maTang)
        {
            return await _context.Tangs
                .Include(t => t.Phongs)
                .Where(t => t.MaTang == maTang)
                .Select(t => new TangDTO
                {
                    MaTang = t.MaTang,
                    TenTang = t.TenTang,
                    MoTa = t.MoTa,
                    SoPhong = t.Phongs != null ? t.Phongs.Count : 0
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<TangDTO> data, int total)> SearchTangsAsync(SearchTangDTO searchDTO)
        {
            var query = _context.Tangs.Include(t => t.Phongs).AsQueryable();

            // Tìm kiếm theo tên tầng
            if (!string.IsNullOrWhiteSpace(searchDTO.TenTang))
            {
                var tenTang = searchDTO.TenTang.ToLower().Trim();
                query = query.Where(t => t.TenTang != null && t.TenTang.ToLower().Contains(tenTang));
            }

            // Đếm tổng số bản ghi
            var total = await query.CountAsync();

            // Phân trang
            var data = await query
                .OrderBy(t => t.MaTang)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(t => new TangDTO
                {
                    MaTang = t.MaTang,
                    TenTang = t.TenTang,
                    MoTa = t.MoTa,
                    SoPhong = t.Phongs != null ? t.Phongs.Count : 0
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<Tang> CreateTangAsync(CreateTangDTO createTangDTO)
        {
            var tang = new Tang
            {
                TenTang = createTangDTO.TenTang,
                MoTa = createTangDTO.MoTa
            };

            _context.Tangs.Add(tang);
            await _context.SaveChangesAsync();
            return tang;
        }

        public async Task<bool> UpdateTangAsync(int maTang, UpdateTangDTO updateTangDTO)
        {
            var tang = await _context.Tangs.FindAsync(maTang);
            if (tang == null) return false;

            if (updateTangDTO.TenTang != null) tang.TenTang = updateTangDTO.TenTang;
            if (updateTangDTO.MoTa != null) tang.MoTa = updateTangDTO.MoTa;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTangAsync(int maTang)
        {
            var tang = await _context.Tangs.FindAsync(maTang);
            if (tang == null) return false;

            // Kiểm tra còn phòng không
            var hasPhongs = await _context.Phongs.AnyAsync(p => p.MaTang == maTang);
            if (hasPhongs) return false;

            _context.Tangs.Remove(tang);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TangExistsAsync(int maTang)
        {
            return await _context.Tangs.AnyAsync(t => t.MaTang == maTang);
        }
    }
}