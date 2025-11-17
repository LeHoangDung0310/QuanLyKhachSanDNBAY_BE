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
            var entity = await _context.TienNghis
                .Include(t => t.Phong_TienNghis)
                .FirstOrDefaultAsync(t => t.MaTienNghi == maTienNghi);

            if (entity == null) return null;

            return new TienNghiDTO
            {
                MaTienNghi = entity.MaTienNghi,
                Ten = entity.Ten,
                Icon = entity.Icon,                 // đảm bảo lấy đúng trường Icon từ DB
                SoPhongSuDung = entity.Phong_TienNghis?.Count ?? 0
            };
        }

        public async Task<(IEnumerable<TienNghiDTO> data, int total)> SearchTienNghisAsync(SearchTienNghiDTO searchDTO)
        {
            var query = _context.TienNghis.AsQueryable();
            // ... filter tên, phân trang ...

            var total = await query.CountAsync();

            var data = await query
                .Include(t => t.Phong_TienNghis)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(t => new TienNghiDTO
                {
                    MaTienNghi = t.MaTienNghi,
                    Ten = t.Ten,
                    Icon = t.Icon,                   // <- chỗ này
                    SoPhongSuDung = t.Phong_TienNghis!.Count
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<TienNghi> CreateTienNghiAsync(CreateTienNghiDTO dto)
        {
            var entity = new TienNghi
            {
                Ten = dto.Ten,
                Icon = dto.Icon      // dto.Icon chính là "/uploads/tiennghi/xxx.png"
            };

            _context.TienNghis.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateTienNghiAsync(int id, UpdateTienNghiDTO dto)
        {
            var entity = await _context.TienNghis.FindAsync(id);
            if (entity == null) return false;

            entity.Ten = dto.Ten;
            if (dto.Icon != null)      // cho phép giữ icon cũ nếu FE gửi null
            {
                entity.Icon = dto.Icon;
            }

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