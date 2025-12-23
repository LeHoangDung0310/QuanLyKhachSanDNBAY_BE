using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.Phong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class PhongRepository : IPhongRepository
    {
        private readonly MyDbContext _context;

        public PhongRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhongDTO>> GetAllPhongsAsync()
        {
            return await _context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .Select(p => new PhongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    MoTa = p.MoTa,
                    TrangThai = p.TrangThai,
                    MaTang = p.MaTang,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null,
                    MaLoaiPhong = p.MaLoaiPhong,
                    TenLoaiPhong = p.LoaiPhong != null ? p.LoaiPhong.TenLoaiPhong : null
                })
                .ToListAsync();
        }

        public async Task<PhongDTO?> GetPhongByIdAsync(int maPhong)
        {
            return await _context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .Where(p => p.MaPhong == maPhong)
                .Select(p => new PhongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    MoTa = p.MoTa,
                    TrangThai = p.TrangThai,
                    MaTang = p.MaTang,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null,
                    MaLoaiPhong = p.MaLoaiPhong,
                    TenLoaiPhong = p.LoaiPhong != null ? p.LoaiPhong.TenLoaiPhong : null,
                    GiaMoiDem = p.LoaiPhong != null ? p.LoaiPhong.GiaMoiDem : null,
                    SoNguoiToiDa = p.LoaiPhong != null ? p.LoaiPhong.SoNguoiToiDa : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<PhongDTO> data, int total)> SearchPhongsAsync(
            SearchPhongDTO searchDTO,
            DateTime ngayNhanPhong,
            DateTime ngayTraPhong)
        {
            var start = ngayNhanPhong.Date;
            var end = ngayTraPhong.Date;

            var query = _context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .Where(p => p.TrangThai != "BaoTri")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDTO.SoPhong))
            {
                query = query.Where(p => p.SoPhong!.Contains(searchDTO.SoPhong));
            }

            if (searchDTO.MaLoaiPhong.HasValue)
            {
                query = query.Where(p => p.MaLoaiPhong == searchDTO.MaLoaiPhong);
            }

            if (searchDTO.MaTang.HasValue)
            {
                query = query.Where(p => p.MaTang == searchDTO.MaTang);
            }

            // 🔥 LẤY PHÒNG BỊ CHIẾM THEO NGÀY
            var phongBiChiếm = await _context.DatPhong_Phongs
                .Include(dp => dp.DatPhong)
                .Where(dp =>
                    dp.DatPhong.TrangThai != "DaHuy" &&
                    dp.DatPhong.NgayNhanPhong.Date < end &&
                    dp.DatPhong.NgayTraPhong.Date > start
                )
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToListAsync();

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(p => p.SoPhong)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(p => new PhongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    MaTang = p.MaTang,
                    TenTang = p.Tang!.TenTang,
                    MaLoaiPhong = p.MaLoaiPhong,
                    TenLoaiPhong = p.LoaiPhong!.TenLoaiPhong,
                    GiaMoiDem = p.LoaiPhong!.GiaMoiDem,
                    SoNguoiToiDa = p.LoaiPhong!.SoNguoiToiDa,

                    // ✅ TRẠNG THÁI ĐỘNG
                    TrangThai = phongBiChiếm.Contains(p.MaPhong)
                        ? "DangSuDung"
                        : "Trong"
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<Phong> CreatePhongAsync(CreatePhongDTO createPhongDTO)
        {
            var phong = new Phong
            {
                SoPhong = createPhongDTO.SoPhong,
                MoTa = createPhongDTO.MoTa,
                MaTang = createPhongDTO.MaTang,
                MaLoaiPhong = createPhongDTO.MaLoaiPhong,
                TrangThai = "Trong"
            };

            _context.Phongs.Add(phong);
            await _context.SaveChangesAsync();
            return phong;
        }

        public async Task<bool> UpdatePhongAsync(int maPhong, UpdatePhongDTO updatePhongDTO)
        {
            var phong = await _context.Phongs.FindAsync(maPhong);
            if (phong == null) return false;

            if (updatePhongDTO.SoPhong != null) phong.SoPhong = updatePhongDTO.SoPhong;
            if (updatePhongDTO.MoTa != null) phong.MoTa = updatePhongDTO.MoTa;
            if (updatePhongDTO.TrangThai != null) phong.TrangThai = updatePhongDTO.TrangThai;
            if (updatePhongDTO.MaTang.HasValue) phong.MaTang = updatePhongDTO.MaTang;
            if (updatePhongDTO.MaLoaiPhong.HasValue) phong.MaLoaiPhong = updatePhongDTO.MaLoaiPhong;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePhongAsync(int maPhong)
        {
            var phong = await _context.Phongs.FindAsync(maPhong);
            if (phong == null) return false;

            _context.Phongs.Remove(phong);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PhongExistsAsync(int maPhong)
        {
            return await _context.Phongs.AnyAsync(p => p.MaPhong == maPhong);
        }

        public async Task<bool> SoPhongExistsAsync(string soPhong)
        {
            return await _context.Phongs.AnyAsync(p => p.SoPhong == soPhong);
        }

        // ✅ IMPLEMENT METHOD MỚI - LOGIC ĐÚNG
        public async Task<IEnumerable<PhongTrongDTO>> GetPhongTrongAsync(
            DateTime ngayNhanPhong,
            DateTime ngayTraPhong)
        {
            var start = ngayNhanPhong.Date;
            var end = ngayTraPhong.Date;

            var phongBiChiếm = await _context.DatPhong_Phongs
                .Include(dp => dp.DatPhong)
                .Where(dp =>
                    dp.DatPhong.TrangThai != "DaHuy" &&
                    dp.DatPhong.NgayNhanPhong.Date < end &&
                    dp.DatPhong.NgayTraPhong.Date > start
                )
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToListAsync();

            var result = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Include(p => p.Tang)
                .Where(p =>
                    p.TrangThai != "BaoTri" &&
                    !phongBiChiếm.Contains(p.MaPhong)
                )
                .Select(p => new PhongTrongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    TrangThai = "Trong",
                    MaLoaiPhong = p.MaLoaiPhong ?? 0,
                    TenLoaiPhong = p.LoaiPhong!.TenLoaiPhong,
                    GiaMoiDem = p.LoaiPhong!.GiaMoiDem,
                    SoNguoiToiDa = p.LoaiPhong!.SoNguoiToiDa,
                    DienTich = p.LoaiPhong!.DienTich,
                    MoTa = p.LoaiPhong!.MoTa,
                    TenTang = p.Tang!.TenTang
                })
                .OrderBy(p => p.SoPhong)
                .ToListAsync();

            return result;
        }
    }
}