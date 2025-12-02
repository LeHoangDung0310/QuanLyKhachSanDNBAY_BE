using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class HinhAnhLPhongRepository : IHinhAnhLPhongRepository
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HinhAnhLPhongRepository(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<HinhAnhLPhongDTO>> GetAllHinhAnhLPhongsAsync()
        {
            return await _context.HinhAnhLPhongs
                .Include(h => h.LoaiPhong)
                .Select(h => new HinhAnhLPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaLoaiPhong = h.MaLoaiPhong,
                    TenLoaiPhong = h.LoaiPhong!.TenLoaiPhong,
                    Url = h.Url ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<HinhAnhLPhongDTO?> GetHinhAnhLPhongByIdAsync(int id)
        {
            return await _context.HinhAnhLPhongs
                .Include(h => h.LoaiPhong)
                .Where(h => h.MaHinhAnh == id)
                .Select(h => new HinhAnhLPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaLoaiPhong = h.MaLoaiPhong,
                    TenLoaiPhong = h.LoaiPhong!.TenLoaiPhong,
                    Url = h.Url ?? string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HinhAnhLPhongDTO>> GetHinhAnhsByLoaiPhongIdAsync(int maLoaiPhong)
        {
            return await _context.HinhAnhLPhongs
                .Include(h => h.LoaiPhong)
                .Where(h => h.MaLoaiPhong == maLoaiPhong)
                .Select(h => new HinhAnhLPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaLoaiPhong = h.MaLoaiPhong,
                    TenLoaiPhong = h.LoaiPhong!.TenLoaiPhong,
                    Url = h.Url ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<(IEnumerable<HinhAnhLPhongDTO> data, int total)> SearchHinhAnhLPhongsAsync(SearchHinhAnhLPhongDTO searchDTO)
        {
            var query = _context.HinhAnhLPhongs
                .Include(h => h.LoaiPhong)
                .AsQueryable();

            if (searchDTO.MaLoaiPhong.HasValue)
            {
                query = query.Where(h => h.MaLoaiPhong == searchDTO.MaLoaiPhong.Value);
            }

            if (!string.IsNullOrEmpty(searchDTO.TenLoaiPhong))
            {
                query = query.Where(h => h.LoaiPhong!.TenLoaiPhong.Contains(searchDTO.TenLoaiPhong));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(h => h.MaLoaiPhong)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(h => new HinhAnhLPhongDTO
                {
                    MaHinhAnh = h.MaHinhAnh,
                    MaLoaiPhong = h.MaLoaiPhong,
                    TenLoaiPhong = h.LoaiPhong!.TenLoaiPhong,
                    Url = h.Url ?? string.Empty
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<HinhAnhLPhong> CreateHinhAnhLPhongAsync(CreateHinhAnhLPhongDTO createHinhAnhLPhongDTO)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "loaiphong");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + createHinhAnhLPhongDTO.File.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await createHinhAnhLPhongDTO.File.CopyToAsync(fileStream);
            }

            var hinhAnhLPhong = new HinhAnhLPhong
            {
                MaLoaiPhong = createHinhAnhLPhongDTO.MaLoaiPhong,
                Url = "/uploads/loaiphong/" + uniqueFileName
            };

            _context.HinhAnhLPhongs.Add(hinhAnhLPhong);
            await _context.SaveChangesAsync();

            return hinhAnhLPhong;
        }

        public async Task<bool> UpdateHinhAnhLPhongAsync(int id, UpdateHinhAnhLPhongDTO updateHinhAnhLPhongDTO)
        {
            var hinhAnhLPhong = await _context.HinhAnhLPhongs.FindAsync(id);
            if (hinhAnhLPhong == null)
            {
                return false;
            }

            // Hiện tại không có gì để update
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHinhAnhLPhongAsync(int id)
        {
            var hinhAnhLPhong = await _context.HinhAnhLPhongs.FindAsync(id);
            if (hinhAnhLPhong == null)
            {
                return false;
            }

            // Xóa file vật lý
            if (!string.IsNullOrEmpty(hinhAnhLPhong.Url))
            {
                var filePath = Path.Combine(_environment.WebRootPath, hinhAnhLPhong.Url.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _context.HinhAnhLPhongs.Remove(hinhAnhLPhong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HinhAnhLPhongExistsAsync(int id)
        {
            return await _context.HinhAnhLPhongs.AnyAsync(e => e.MaHinhAnh == id);
        }
    }
}