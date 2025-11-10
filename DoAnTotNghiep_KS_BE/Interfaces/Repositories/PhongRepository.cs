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
                .Select(p => new PhongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    TenLoai = p.TenLoai,
                    DienTich = p.DienTich,
                    SoGiuong = p.SoGiuong,
                    SoNguoiToiDa = p.SoNguoiToiDa,
                    HuongNhin = p.HuongNhin,
                    MoTa = p.MoTa,
                    GiaMoiDem = p.GiaMoiDem,
                    TrangThai = p.TrangThai,
                    MaTang = p.MaTang,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null
                })
                .ToListAsync();
        }

        public async Task<PhongDTO?> GetPhongByIdAsync(int maPhong)
        {
            return await _context.Phongs
                .Include(p => p.Tang)
                .Where(p => p.MaPhong == maPhong)
                .Select(p => new PhongDTO
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    TenLoai = p.TenLoai,
                    DienTich = p.DienTich,
                    SoGiuong = p.SoGiuong,
                    SoNguoiToiDa = p.SoNguoiToiDa,
                    HuongNhin = p.HuongNhin,
                    MoTa = p.MoTa,
                    GiaMoiDem = p.GiaMoiDem,
                    TrangThai = p.TrangThai,
                    MaTang = p.MaTang,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<PhongDTO> data, int total)> SearchPhongsAsync(SearchPhongDTO searchDTO)
        {
            var query = _context.Phongs.Include(p => p.Tang).AsQueryable();

            // Tìm kiếm theo số phòng
            if (!string.IsNullOrWhiteSpace(searchDTO.SoPhong))
            {
                var soPhong = searchDTO.SoPhong.ToLower().Trim();
                query = query.Where(p => p.SoPhong != null && p.SoPhong.ToLower().Contains(soPhong));
            }

            // Tìm kiếm theo tên loại
            if (!string.IsNullOrWhiteSpace(searchDTO.TenLoai))
            {
                var tenLoai = searchDTO.TenLoai.ToLower().Trim();
                query = query.Where(p => p.TenLoai != null && p.TenLoai.ToLower().Contains(tenLoai));
            }

            // Lọc theo số giường
            if (searchDTO.SoGiuongMin.HasValue)
            {
                query = query.Where(p => p.SoGiuong >= searchDTO.SoGiuongMin.Value);
            }
            if (searchDTO.SoGiuongMax.HasValue)
            {
                query = query.Where(p => p.SoGiuong <= searchDTO.SoGiuongMax.Value);
            }

            // Lọc theo số người tối đa
            if (searchDTO.SoNguoiToiDaMin.HasValue)
            {
                query = query.Where(p => p.SoNguoiToiDa >= searchDTO.SoNguoiToiDaMin.Value);
            }
            if (searchDTO.SoNguoiToiDaMax.HasValue)
            {
                query = query.Where(p => p.SoNguoiToiDa <= searchDTO.SoNguoiToiDaMax.Value);
            }

            // Lọc theo giá
            if (searchDTO.GiaMin.HasValue)
            {
                query = query.Where(p => p.GiaMoiDem >= searchDTO.GiaMin.Value);
            }
            if (searchDTO.GiaMax.HasValue)
            {
                query = query.Where(p => p.GiaMoiDem <= searchDTO.GiaMax.Value);
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
                    TenLoai = p.TenLoai,
                    DienTich = p.DienTich,
                    SoGiuong = p.SoGiuong,
                    SoNguoiToiDa = p.SoNguoiToiDa,
                    HuongNhin = p.HuongNhin,
                    MoTa = p.MoTa,
                    GiaMoiDem = p.GiaMoiDem,
                    TrangThai = p.TrangThai,
                    MaTang = p.MaTang,
                    TenTang = p.Tang != null ? p.Tang.TenTang : null
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<Phong> CreatePhongAsync(CreatePhongDTO createPhongDTO)
        {
            var phong = new Phong
            {
                SoPhong = createPhongDTO.SoPhong,
                TenLoai = createPhongDTO.TenLoai,
                DienTich = createPhongDTO.DienTich,
                SoGiuong = createPhongDTO.SoGiuong,
                SoNguoiToiDa = createPhongDTO.SoNguoiToiDa,
                HuongNhin = createPhongDTO.HuongNhin,
                MoTa = createPhongDTO.MoTa,
                GiaMoiDem = createPhongDTO.GiaMoiDem,
                MaTang = createPhongDTO.MaTang,
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
            if (updatePhongDTO.TenLoai != null) phong.TenLoai = updatePhongDTO.TenLoai;
            if (updatePhongDTO.DienTich.HasValue) phong.DienTich = updatePhongDTO.DienTich;
            if (updatePhongDTO.SoGiuong.HasValue) phong.SoGiuong = updatePhongDTO.SoGiuong;
            if (updatePhongDTO.SoNguoiToiDa.HasValue) phong.SoNguoiToiDa = updatePhongDTO.SoNguoiToiDa;
            if (updatePhongDTO.HuongNhin != null) phong.HuongNhin = updatePhongDTO.HuongNhin;
            if (updatePhongDTO.MoTa != null) phong.MoTa = updatePhongDTO.MoTa;
            if (updatePhongDTO.GiaMoiDem.HasValue) phong.GiaMoiDem = updatePhongDTO.GiaMoiDem;
            if (updatePhongDTO.TrangThai != null) phong.TrangThai = updatePhongDTO.TrangThai;
            if (updatePhongDTO.MaTang.HasValue) phong.MaTang = updatePhongDTO.MaTang;

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
    }
}