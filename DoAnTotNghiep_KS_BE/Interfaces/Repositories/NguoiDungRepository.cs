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
                    NgayTao = n.NgayTao
                })
                .OrderByDescending(n => n.NgayTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<NguoiDungDTO>> GetNguoiDungsByRoleAsync(string vaiTro)
        {
            return await _context.NguoiDungs
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .Where(n => n.VaiTro == vaiTro)
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
                    NgayTao = n.NgayTao
                })
                .OrderByDescending(n => n.NgayTao)
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
                    NgayTao = n.NgayTao
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

            if (!string.IsNullOrWhiteSpace(searchDTO.SearchTerm))
            {
                var term = searchDTO.SearchTerm.Trim().ToLower();
                query = query.Where(n =>
                    n.Email.ToLower().Contains(term) ||
                    (n.HoTen != null && n.HoTen.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(searchDTO.VaiTro))
            {
                query = query.Where(n => n.VaiTro == searchDTO.VaiTro);
            }

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
                    NgayTao = n.NgayTao
                })
                .ToListAsync();

            return (data, total);
        }

        public async Task<bool> UpdateNguoiDungAsync(int maNguoiDung, UpdateNguoiDungAdminDTO updateDTO)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);
            if (nguoiDung == null)
            {
                return false;
            }

            // Cập nhật các trường không null
            if (!string.IsNullOrWhiteSpace(updateDTO.HoTen))
            {
                nguoiDung.HoTen = updateDTO.HoTen.Trim();
            }

            if (!string.IsNullOrWhiteSpace(updateDTO.SoDienThoai))
            {
                nguoiDung.SoDienThoai = updateDTO.SoDienThoai.Trim();
            }

            if (updateDTO.DiaChiChiTiet != null)
            {
                nguoiDung.DiaChiChiTiet = string.IsNullOrWhiteSpace(updateDTO.DiaChiChiTiet)
                    ? null
                    : updateDTO.DiaChiChiTiet.Trim();
            }

            // FIX: Cập nhật MaPhuongXa ĐÚNG CÁCH
            // Nếu có giá trị trong DTO (kể cả null), thì cập nhật
            nguoiDung.MaPhuongXa = updateDTO.MaPhuongXa;

            if (!string.IsNullOrWhiteSpace(updateDTO.VaiTro))
            {
                var norm = updateDTO.VaiTro.Trim().ToLower();

                string? mappedRole = norm switch
                {
                    "admin" => "Admin",
                    "khachhang" => "KhachHang",
                    "letan" => "LeTan",
                    _ => null
                };

                if (mappedRole != null)
                {
                    nguoiDung.VaiTro = mappedRole;
                }
            }

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

            if (!string.IsNullOrWhiteSpace(updateDTO.SoDienThoai))
            {
                nguoiDung.SoDienThoai = updateDTO.SoDienThoai.Trim();
            }
            else
            {
                nguoiDung.SoDienThoai = null;
            }

            nguoiDung.DiaChiChiTiet = string.IsNullOrWhiteSpace(updateDTO.DiaChiChiTiet)
                ? null
                : updateDTO.DiaChiChiTiet.Trim();

            nguoiDung.MaPhuongXa = updateDTO.MaPhuongXa;

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

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.MatKhauCu, nguoiDung.MatKhau))
            {
                return (false, "Mật khẩu hiện tại không đúng");
            }

            // Hash new password
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