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
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .OrderByDescending(n => n.NgayTao)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChiChiTiet = n.DiaChiChiTiet,
                    MaPhuongXa = n.MaPhuongXa,
                    TenPhuongXa = n.PhuongXa != null ? n.PhuongXa.TenPhuongXa : null,
                    MaHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.MaHuyen : null,
                    TenHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.TenHuyen : null,
                    MaTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.MaTinh : null,
                    TenTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.TenTinh : null,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao,
                    SoCCCD = n.SoCCCD,
                    NgayCapCCCD = n.NgayCapCCCD,
                    NoiCapCCCD = n.NoiCapCCCD,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDungDTO>> GetNguoiDungsByRoleAsync(string vaiTro)
        {
            return await _context.NguoiDungs
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .Where(n => n.VaiTro == vaiTro)
                .OrderByDescending(n => n.NgayTao)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChiChiTiet = n.DiaChiChiTiet,
                    MaPhuongXa = n.MaPhuongXa,
                    TenPhuongXa = n.PhuongXa != null ? n.PhuongXa.TenPhuongXa : null,
                    MaHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.MaHuyen : null,
                    TenHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.TenHuyen : null,
                    MaTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.MaTinh : null,
                    TenTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.TenTinh : null,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao,
                    SoCCCD = n.SoCCCD,
                    NgayCapCCCD = n.NgayCapCCCD,
                    NoiCapCCCD = n.NoiCapCCCD,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh
                })
                .ToListAsync();
        }

        public async Task<NguoiDungDTO?> GetNguoiDungByIdAsync(int maNguoiDung)
        {
            return await _context.NguoiDungs
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .Where(n => n.MaNguoiDung == maNguoiDung)
                .Select(n => new NguoiDungDTO
                {
                    MaNguoiDung = n.MaNguoiDung,
                    Email = n.Email,
                    VaiTro = n.VaiTro,
                    HoTen = n.HoTen,
                    SoDienThoai = n.SoDienThoai,
                    DiaChiChiTiet = n.DiaChiChiTiet,
                    MaPhuongXa = n.MaPhuongXa,
                    TenPhuongXa = n.PhuongXa != null ? n.PhuongXa.TenPhuongXa : null,
                    MaHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.MaHuyen : null,
                    TenHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.TenHuyen : null,
                    MaTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.MaTinh : null,
                    TenTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.TenTinh : null,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao,
                    SoCCCD = n.SoCCCD,
                    NgayCapCCCD = n.NgayCapCCCD,
                    NoiCapCCCD = n.NoiCapCCCD,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<NguoiDungDTO> data, int total)> SearchNguoiDungsAsync(SearchNguoiDungDTO searchDTO)
        {
            var query = _context.NguoiDungs
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(searchDTO.SearchTerm))
            {
                var term = searchDTO.SearchTerm.Trim().ToLower();
                query = query.Where(n =>
                    n.Email.ToLower().Contains(term) ||
                    (n.HoTen != null && n.HoTen.ToLower().Contains(term)) ||
                    (n.SoCCCD != null && n.SoCCCD.Contains(term)) ||
                    (n.SoDienThoai != null && n.SoDienThoai.Contains(term)));
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

            var total = await query.CountAsync();

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
                    DiaChiChiTiet = n.DiaChiChiTiet,
                    MaPhuongXa = n.MaPhuongXa,
                    TenPhuongXa = n.PhuongXa != null ? n.PhuongXa.TenPhuongXa : null,
                    MaHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.MaHuyen : null,
                    TenHuyen = n.PhuongXa != null && n.PhuongXa.Huyen != null ? n.PhuongXa.Huyen.TenHuyen : null,
                    MaTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.MaTinh : null,
                    TenTinh = n.PhuongXa != null && n.PhuongXa.Huyen != null && n.PhuongXa.Huyen.Tinh != null ? n.PhuongXa.Huyen.Tinh.TenTinh : null,
                    AnhDaiDien = n.AnhDaiDien,
                    TrangThai = n.TrangThai,
                    NgayTao = n.NgayTao,
                    SoCCCD = n.SoCCCD,
                    NgayCapCCCD = n.NgayCapCCCD,
                    NoiCapCCCD = n.NoiCapCCCD,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<bool> UpdateNguoiDungAsync(int maNguoiDung, UpdateNguoiDungAdminDTO updateDTO)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null) return false;

            // Cập nhật các trường cơ bản
            if (!string.IsNullOrWhiteSpace(updateDTO.HoTen))
            {
                nguoiDung.HoTen = updateDTO.HoTen.Trim();
            }

            nguoiDung.SoDienThoai = string.IsNullOrWhiteSpace(updateDTO.SoDienThoai)
                ? null
                : updateDTO.SoDienThoai.Trim();

            nguoiDung.DiaChiChiTiet = string.IsNullOrWhiteSpace(updateDTO.DiaChiChiTiet)
                ? null
                : updateDTO.DiaChiChiTiet.Trim();

            nguoiDung.MaPhuongXa = updateDTO.MaPhuongXa;

            // Cập nhật thông tin CCCD
            nguoiDung.SoCCCD = string.IsNullOrWhiteSpace(updateDTO.SoCCCD)
                ? null
                : updateDTO.SoCCCD.Trim();

            nguoiDung.NgayCapCCCD = updateDTO.NgayCapCCCD;

            nguoiDung.NoiCapCCCD = string.IsNullOrWhiteSpace(updateDTO.NoiCapCCCD)
                ? null
                : updateDTO.NoiCapCCCD.Trim();

            // Cập nhật thông tin cá nhân
            nguoiDung.NgaySinh = updateDTO.NgaySinh;

            nguoiDung.GioiTinh = string.IsNullOrWhiteSpace(updateDTO.GioiTinh)
                ? null
                : updateDTO.GioiTinh.Trim();

            // Cập nhật vai trò
            if (!string.IsNullOrWhiteSpace(updateDTO.VaiTro))
            {
                nguoiDung.VaiTro = updateDTO.VaiTro.Trim();
            }

            // Cập nhật trạng thái
            if (!string.IsNullOrWhiteSpace(updateDTO.TrangThai))
            {
                nguoiDung.TrangThai = updateDTO.TrangThai;
            }

            try
            {
                _context.NguoiDungs.Update(nguoiDung);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật người dùng: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProfileAsync(int maNguoiDung, UpdateProfileDTO updateDTO)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null) return false;

            if (!string.IsNullOrWhiteSpace(updateDTO.HoTen))
            {
                nguoiDung.HoTen = updateDTO.HoTen.Trim();
            }

            nguoiDung.SoDienThoai = string.IsNullOrWhiteSpace(updateDTO.SoDienThoai)
                ? null
                : updateDTO.SoDienThoai.Trim();

            nguoiDung.DiaChiChiTiet = string.IsNullOrWhiteSpace(updateDTO.DiaChiChiTiet)
                ? null
                : updateDTO.DiaChiChiTiet.Trim();

            nguoiDung.MaPhuongXa = updateDTO.MaPhuongXa;

            // Cập nhật thông tin CCCD
            nguoiDung.SoCCCD = string.IsNullOrWhiteSpace(updateDTO.SoCCCD)
                ? null
                : updateDTO.SoCCCD.Trim();

            nguoiDung.NgayCapCCCD = updateDTO.NgayCapCCCD;

            nguoiDung.NoiCapCCCD = string.IsNullOrWhiteSpace(updateDTO.NoiCapCCCD)
                ? null
                : updateDTO.NoiCapCCCD.Trim();

            // Cập nhật thông tin cá nhân
            nguoiDung.NgaySinh = updateDTO.NgaySinh;

            nguoiDung.GioiTinh = string.IsNullOrWhiteSpace(updateDTO.GioiTinh)
                ? null
                : updateDTO.GioiTinh.Trim();

            try
            {
                _context.NguoiDungs.Update(nguoiDung);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int maNguoiDung, ChangePasswordDTO changePasswordDTO)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return (false, "Người dùng không tồn tại");
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.MatKhauCu, nguoiDung.MatKhau))
            {
                return (false, "Mật khẩu hiện tại không đúng");
            }

            nguoiDung.MatKhau = BCrypt.Net.BCrypt.HashPassword(changePasswordDTO.MatKhauMoi);

            try
            {
                _context.NguoiDungs.Update(nguoiDung);
                await _context.SaveChangesAsync();
                return (true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return (false, "Có lỗi xảy ra khi đổi mật khẩu");
            }
        }

        public async Task<bool> DeleteNguoiDungAsync(int maNguoiDung)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (nguoiDung == null) return false;

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
            var hasDatPhong = await _context.DatPhongs.AnyAsync(d => d.MaKhachHang == maNguoiDung);
            if (hasDatPhong) return false;

            var hasHuyDatPhong = await _context.HuyDatPhongs.AnyAsync(h => h.MaNguoiDuyet == maNguoiDung);
            if (hasHuyDatPhong) return false;

            var hasHoanTien = await _context.HoanTiens.AnyAsync(h => h.MaQuanTri == maNguoiDung);
            if (hasHoanTien) return false;

            var hasDanhGia = await _context.DanhGias.AnyAsync(d => d.MaKhachHang == maNguoiDung);
            if (hasDanhGia) return false;

            return true;
        }

        public async Task<bool> UpdateAvatarAsync(int maNguoiDung, string avatarUrl)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null) return false;

            nguoiDung.AnhDaiDien = avatarUrl;

            try
            {
                _context.NguoiDungs.Update(nguoiDung);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating avatar: {ex.Message}");
                return false;
            }
        }
    }
}