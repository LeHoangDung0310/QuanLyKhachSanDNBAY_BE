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
                    TenLoaiPhong = p.LoaiPhong != null ? p.LoaiPhong.TenLoaiPhong : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<PhongDTO> data, int total)> SearchPhongsAsync(SearchPhongDTO searchDTO)
        {
            var query = _context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .AsQueryable();

            // Tìm kiếm theo số phòng
            if (!string.IsNullOrWhiteSpace(searchDTO.SoPhong))
            {
                var soPhong = searchDTO.SoPhong.ToLower().Trim();
                query = query.Where(p => p.SoPhong != null && p.SoPhong.ToLower().Contains(soPhong));
            }

            // Lọc theo loại phòng
            if (searchDTO.MaLoaiPhong.HasValue)
            {
                query = query.Where(p => p.MaLoaiPhong == searchDTO.MaLoaiPhong.Value);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(searchDTO.TrangThai))
            {
                query = query.Where(p => p.TrangThai == searchDTO.TrangThai);
            }

            // Lọc theo tầng
            if (searchDTO.MaTang.HasValue)
            {
                query = query.Where(p => p.MaTang == searchDTO.MaTang.Value);
            }

            // Đếm tổng số bản ghi
            var total = await query.CountAsync();

            // Phân trang
            var data = await query
                .OrderBy(p => p.SoPhong)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
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
        public async Task<IEnumerable<PhongTrongDTO>> GetPhongTrongAsync(DateTime ngayNhanPhong, DateTime ngayTraPhong)
        {
            // 1. Lấy TẤT CẢ phòng (không lọc trạng thái vì phòng đang dùng hôm nay có thể trống ngày mai)
            var tatCaPhong = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Include(p => p.Tang)
                .Where(p => p.TrangThai != "BaoTri") // Chỉ loại phòng đang bảo trì
                .ToListAsync();

            // 2. Lấy danh sách phòng đã được đặt TRÙNG LỊCH trong khoảng thời gian
            var phongDaDatTrungLich = await _context.DatPhong_Phongs
                .Include(dp => dp.DatPhong)
                .Where(dp =>
                    // Chỉ tính booking chưa hủy và chưa hoàn tất
                    dp.DatPhong.TrangThai != "DaHuy" &&
                    dp.DatPhong.TrangThai != "DaThanhToan" &&
                    // Kiểm tra trùng thời gian (overlap)
                    dp.DatPhong.NgayNhanPhong < ngayTraPhong &&
                    dp.DatPhong.NgayTraPhong > ngayNhanPhong
                )
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToListAsync();

            // 3. Phòng khả dụng = Tất cả phòng - Phòng có booking trùng lịch
            var phongKhaDung = tatCaPhong
                .Where(p => !phongDaDatTrungLich.Contains(p.MaPhong))
                .Select(p => new PhongTrongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    TrangThai = p.TrangThai,
                    TenLoaiPhong = p.LoaiPhong != null ? p.LoaiPhong.TenLoaiPhong : null,
                    GiaMoiDem = p.LoaiPhong != null ? p.LoaiPhong.GiaMoiDem : null,
                    SoNguoiToiDa = p.LoaiPhong != null ? p.LoaiPhong.SoNguoiToiDa : null,
                    DienTich = p.LoaiPhong != null ? p.LoaiPhong.DienTich : null,
                    MoTa = p.LoaiPhong != null ? p.LoaiPhong.MoTa : null,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null,
                    MaLoaiPhong = p.MaLoaiPhong ?? 0
                })
                .OrderBy(p => p.SoPhong)
                .ToList();

            return phongKhaDung;
        }
    }
}