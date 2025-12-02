using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class LoaiPhongRepository : ILoaiPhongRepository
    {
        private readonly MyDbContext _context;

        public LoaiPhongRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoaiPhongDTO>> GetAllLoaiPhongsAsync()
        {
            return await _context.LoaiPhongs
                .Select(lp => new LoaiPhongDTO
                {
                    MaLoaiPhong = lp.MaLoaiPhong,
                    TenLoaiPhong = lp.TenLoaiPhong,
                    DienTich = lp.DienTich,
                    SoGiuong = lp.SoGiuong,
                    SoNguoiToiDa = lp.SoNguoiToiDa,
                    GiaMoiDem = lp.GiaMoiDem,
                    MoTa = lp.MoTa
                })
                .ToListAsync();
        }

        public async Task<LoaiPhongDTO?> GetLoaiPhongByIdAsync(int maLoaiPhong)
        {
            return await _context.LoaiPhongs
                .Where(lp => lp.MaLoaiPhong == maLoaiPhong)
                .Select(lp => new LoaiPhongDTO
                {
                    MaLoaiPhong = lp.MaLoaiPhong,
                    TenLoaiPhong = lp.TenLoaiPhong,
                    DienTich = lp.DienTich,
                    SoGiuong = lp.SoGiuong,
                    SoNguoiToiDa = lp.SoNguoiToiDa,
                    GiaMoiDem = lp.GiaMoiDem,
                    MoTa = lp.MoTa
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<LoaiPhongDTO> data, int total)> SearchLoaiPhongsAsync(SearchLoaiPhongDTO searchDTO)
        {
            var query = _context.LoaiPhongs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDTO.TenLoaiPhong))
            {
                query = query.Where(lp => lp.TenLoaiPhong.Contains(searchDTO.TenLoaiPhong));
            }

            if (searchDTO.SoGiuongMin.HasValue)
            {
                query = query.Where(lp => lp.SoGiuong >= searchDTO.SoGiuongMin.Value);
            }

            if (searchDTO.SoGiuongMax.HasValue)
            {
                query = query.Where(lp => lp.SoGiuong <= searchDTO.SoGiuongMax.Value);
            }

            if (searchDTO.SoNguoiToiDaMin.HasValue)
            {
                query = query.Where(lp => lp.SoNguoiToiDa >= searchDTO.SoNguoiToiDaMin.Value);
            }

            if (searchDTO.SoNguoiToiDaMax.HasValue)
            {
                query = query.Where(lp => lp.SoNguoiToiDa <= searchDTO.SoNguoiToiDaMax.Value);
            }

            if (searchDTO.GiaMin.HasValue)
            {
                query = query.Where(lp => lp.GiaMoiDem >= searchDTO.GiaMin.Value);
            }

            if (searchDTO.GiaMax.HasValue)
            {
                query = query.Where(lp => lp.GiaMoiDem <= searchDTO.GiaMax.Value);
            }

            var total = await query.CountAsync();

            var data = await query
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(lp => new LoaiPhongDTO
                {
                    MaLoaiPhong = lp.MaLoaiPhong,
                    TenLoaiPhong = lp.TenLoaiPhong,
                    DienTich = lp.DienTich,
                    SoGiuong = lp.SoGiuong,
                    SoNguoiToiDa = lp.SoNguoiToiDa,
                    GiaMoiDem = lp.GiaMoiDem,
                    MoTa = lp.MoTa
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<LoaiPhong> CreateLoaiPhongAsync(CreateLoaiPhongDTO createLoaiPhongDTO)
        {
            var loaiPhong = new LoaiPhong
            {
                TenLoaiPhong = createLoaiPhongDTO.TenLoaiPhong,
                DienTich = createLoaiPhongDTO.DienTich,
                SoGiuong = createLoaiPhongDTO.SoGiuong,
                SoNguoiToiDa = createLoaiPhongDTO.SoNguoiToiDa,
                GiaMoiDem = createLoaiPhongDTO.GiaMoiDem,
                MoTa = createLoaiPhongDTO.MoTa
            };

            _context.LoaiPhongs.Add(loaiPhong);
            await _context.SaveChangesAsync();
            return loaiPhong;
        }

        public async Task<bool> UpdateLoaiPhongAsync(int id, UpdateLoaiPhongDTO updateLoaiPhongDTO)
        {
            var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhong == null) return false;

            loaiPhong.TenLoaiPhong = updateLoaiPhongDTO.TenLoaiPhong;
            loaiPhong.DienTich = updateLoaiPhongDTO.DienTich;
            loaiPhong.SoGiuong = updateLoaiPhongDTO.SoGiuong;
            loaiPhong.SoNguoiToiDa = updateLoaiPhongDTO.SoNguoiToiDa;
            loaiPhong.GiaMoiDem = updateLoaiPhongDTO.GiaMoiDem;
            loaiPhong.MoTa = updateLoaiPhongDTO.MoTa;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLoaiPhongAsync(int id)
        {
            var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
            if (loaiPhong == null) return false;

            _context.LoaiPhongs.Remove(loaiPhong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoaiPhongExistsAsync(int id)
        {
            return await _context.LoaiPhongs.AnyAsync(lp => lp.MaLoaiPhong == id);
        }
    }
}