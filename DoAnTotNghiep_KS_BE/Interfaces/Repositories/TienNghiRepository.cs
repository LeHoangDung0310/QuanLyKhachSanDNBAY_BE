using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class TienNghiRepository : ITienNghiRepository
    {
        private readonly MyDbContext _context;

        public TienNghiRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TienNghiDTO>> GetAllTienNghisAsync()
        {
            return await _context.TienNghis
                .Include(t => t.Phong_TienNghis)
                .Select(t => new TienNghiDTO
                {
                    MaTienNghi = t.MaTienNghi,
                    Ten = t.Ten,
                    Icon = t.Icon,
                    SoPhongSuDung = t.Phong_TienNghis != null ? t.Phong_TienNghis.Count : 0
                })
                .ToListAsync();
        }

        public async Task<TienNghiDTO?> GetTienNghiByIdAsync(int maTienNghi)
        {
            return await _context.TienNghis
                .Include(t => t.Phong_TienNghis)
                .Where(t => t.MaTienNghi == maTienNghi)
                .Select(t => new TienNghiDTO
                {
                    MaTienNghi = t.MaTienNghi,
                    Ten = t.Ten,
                    Icon = t.Icon,
                    SoPhongSuDung = t.Phong_TienNghis != null ? t.Phong_TienNghis.Count : 0
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<TienNghiDTO> data, int total)> SearchTienNghisAsync(SearchTienNghiDTO searchDTO)
        {
            var query = _context.TienNghis.Include(t => t.Phong_TienNghis).AsQueryable();

            // Tìm kiếm theo tên tiện nghi
            if (!string.IsNullOrWhiteSpace(searchDTO.Ten))
            {
                var ten = searchDTO.Ten.ToLower().Trim();
                query = query.Where(t => t.Ten != null && t.Ten.ToLower().Contains(ten));
            }

            // Đếm tổng số bản ghi
            var total = await query.CountAsync();

            // Phân trang
            var data = await query
                .OrderBy(t => t.Ten)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(t => new TienNghiDTO
                {
                    MaTienNghi = t.MaTienNghi,
                    Ten = t.Ten,
                    Icon = t.Icon,
                    SoPhongSuDung = t.Phong_TienNghis != null ? t.Phong_TienNghis.Count : 0
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<TienNghi> CreateTienNghiAsync(CreateTienNghiDTO createTienNghiDTO)
        {
            var tienNghi = new TienNghi
            {
                Ten = createTienNghiDTO.Ten,
                Icon = createTienNghiDTO.Icon
            };

            _context.TienNghis.Add(tienNghi);
            await _context.SaveChangesAsync();
            return tienNghi;
        }

        public async Task<bool> UpdateTienNghiAsync(int maTienNghi, UpdateTienNghiDTO updateTienNghiDTO)
        {
            var tienNghi = await _context.TienNghis.FindAsync(maTienNghi);
            if (tienNghi == null) return false;

            if (updateTienNghiDTO.Ten != null) tienNghi.Ten = updateTienNghiDTO.Ten;
            if (updateTienNghiDTO.Icon != null) tienNghi.Icon = updateTienNghiDTO.Icon;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTienNghiAsync(int maTienNghi)
        {
            var tienNghi = await _context.TienNghis.FindAsync(maTienNghi);
            if (tienNghi == null) return false;

            // Kiểm tra còn phòng sử dụng không
            var isUsed = await _context.Phong_TienNghis.AnyAsync(pt => pt.MaTienNghi == maTienNghi);
            if (isUsed) return false;

            _context.TienNghis.Remove(tienNghi);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TienNghiExistsAsync(int maTienNghi)
        {
            return await _context.TienNghis.AnyAsync(t => t.MaTienNghi == maTienNghi);
        }
    }
}