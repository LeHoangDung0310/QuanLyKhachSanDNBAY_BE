using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly MyDbContext _context;

        public NguoiDungRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NguoiDungDTO>> GetAllNguoiDungsAsync()
        {
            return await _context.NguoiDungs
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao
                })
                .OrderByDescending(n => n.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDungDTO>> GetNguoiDungsByRoleAsync(string vaiTro)
        {
            return await _context.NguoiDungs
                .Where(n => n.VaiTro == vaiTro)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao
                })
                .OrderByDescending(n => n.NgayTao)
                .ToListAsync();
        }

        public async Task<NguoiDungDTO?> GetNguoiDungByIdAsync(int maNguoiDung)
        {
            return await _context.NguoiDungs
                .Where(n => n.MaNguoiDung == maNguoiDung)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<NguoiDungDTO> data, int total)> SearchNguoiDungsAsync(SearchNguoiDungDTO searchDTO)
        {
            var query = _context.NguoiDungs.AsQueryable();

            // Tìm kiếm theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(searchDTO.SearchTerm))
            {
                var searchTerm = searchDTO.SearchTerm.ToLower().Trim();
                query = query.Where(n =>
                    (n.HoTen != null && n.HoTen.ToLower().Contains(searchTerm)) ||
                    n.Email.ToLower().Contains(searchTerm));
            }

            // Lọc theo vai trò
            if (!string.IsNullOrWhiteSpace(searchDTO.VaiTro))
            {
                query = query.Where(n => n.VaiTro == searchDTO.VaiTro);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(searchDTO.TrangThai))
            {
                query = query.Where(n => n.TrangThai == searchDTO.TrangThai);
            }

            // Đếm tổng số bản ghi
            var total = await query.CountAsync();

            // Phân trang
            var data = await query
                .OrderByDescending(n => n.NgayTao)
                .Skip((searchDTO.PageNumber - 1) * searchDTO.PageSize)
                .Take(searchDTO.PageSize)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<bool> UpdateNguoiDungAsync(int maNguoiDung, UpdateNguoiDungAdminDTO updateDTO)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (nguoiDung == null) return false;

            // Cập nhật thông tin
            if (updateDTO.HoTen != null) nguoiDung.HoTen = updateDTO.HoTen;
            if (updateDTO.SoDienThoai != null) nguoiDung.SoDienThoai = updateDTO.SoDienThoai;
            if (updateDTO.DiaChi != null) nguoiDung.DiaChi = updateDTO.DiaChi;
            if (updateDTO.AnhDaiDien != null) nguoiDung.AnhDaiDien = updateDTO.AnhDaiDien;
            if (updateDTO.VaiTro != null) nguoiDung.VaiTro = updateDTO.VaiTro;
            if (updateDTO.TrangThai != null) nguoiDung.TrangThai = updateDTO.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNguoiDungAsync(int maNguoiDung)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (nguoiDung == null) return false;

            // Kiểm tra có thể xóa không (không có dữ liệu liên quan)
            var canDelete = await CanDeleteNguoiDungAsync(maNguoiDung);
            if (!canDelete) return false;

            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NguoiDungExistsAsync(int maNguoiDung)
        {
            return await _context.NguoiDungs.AnyAsync(n => n.MaNguoiDung == maNguoiDung);
        }

        public async Task<bool> CanDeleteNguoiDungAsync(int maNguoiDung)
        {
            // Kiểm tra có đặt phòng không
            var hasDatPhong = await _context.DatPhongs.AnyAsync(d => d.MaKhachHang == maNguoiDung);
            if (hasDatPhong) return false;

            // Kiểm tra có hủy đặt phòng không
            var hasHuyDatPhong = await _context.HuyDatPhongs.AnyAsync(h =>
                h.MaKhachHang == maNguoiDung || h.MaNhanVienDuyet == maNguoiDung);
            if (hasHuyDatPhong) return false;

            // Kiểm tra có hoàn tiền không
            var hasHoanTien = await _context.HoanTiens.AnyAsync(h => h.MaQuanTri == maNguoiDung);
            if (hasHoanTien) return false;

            // Kiểm tra có đánh giá không
            var hasDanhGia = await _context.DanhGias.AnyAsync(d => d.MaKhachHang == maNguoiDung);
            if (hasDanhGia) return false;

            return true;
        }
    }
}