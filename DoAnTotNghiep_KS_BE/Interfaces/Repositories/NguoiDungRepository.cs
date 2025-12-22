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
                .Include(n => n.TaiKhoanNganHangs) // Thêm include này
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
                    GioiTinh = n.GioiTinh,
                    // Thông tin tài khoản ngân hàng (lấy tài khoản đầu tiên nếu có)
                    NganHang = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.NganHang
                        : null,
                    SoTaiKhoan = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.SoTaiKhoan
                        : null,
                    TenChuTK = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.TenChuTK
                        : null
                })
                .FirstOrDefaultAsync();
        }

        // Lấy thông tin người dùng theo email (auto-fill khi đặt phòng trực tiếp)
        public async Task<NguoiDungDTO?> GetNguoiDungByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var emailNorm = email.Trim().ToLower();
            return await _context.NguoiDungs
                .Include(n => n.PhuongXa)
                    .ThenInclude(x => x!.Huyen)
                    .ThenInclude(h => h!.Tinh)
                .Include(n => n.TaiKhoanNganHangs)
                .Where(n => n.Email.ToLower() == emailNorm)
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
                    GioiTinh = n.GioiTinh,
                    NganHang = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.NganHang
                        : null,
                    SoTaiKhoan = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.SoTaiKhoan
                        : null,
                    TenChuTK = n.TaiKhoanNganHangs != null && n.TaiKhoanNganHangs.Any()
                        ? n.TaiKhoanNganHangs.FirstOrDefault()!.TenChuTK
                        : null
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
            // ✅ Load người dùng kèm theo tài khoản ngân hàng
            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.TaiKhoanNganHangs)
                .FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);

            if (nguoiDung == null)
            {
                Console.WriteLine($"❌ Không tìm thấy người dùng {maNguoiDung}");
                return false;
            }

            Console.WriteLine($"✅ Tìm thấy người dùng {maNguoiDung}: {nguoiDung.Email}");

            // Cập nhật thông tin cơ bản
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

            // ✅ XỬ LÝ TÀI KHOẢN NGÂN HÀNG
            Console.WriteLine("🏦 Bắt đầu xử lý tài khoản ngân hàng...");
            Console.WriteLine($"   - Ngân hàng: {updateDTO.NganHang}");
            Console.WriteLine($"   - Số TK: {updateDTO.SoTaiKhoan}");
            Console.WriteLine($"   - Chủ TK: {updateDTO.TenChuTK}");

            // Kiểm tra có dữ liệu ngân hàng mới không
            bool hasNewBankData = !string.IsNullOrWhiteSpace(updateDTO.NganHang) ||
                          !string.IsNullOrWhiteSpace(updateDTO.SoTaiKhoan) ||
                          !string.IsNullOrWhiteSpace(updateDTO.TenChuTK);

            Console.WriteLine($"   - Có dữ liệu ngân hàng mới: {hasNewBankData}");

            // Lấy tài khoản ngân hàng hiện tại (nếu có)
            var taiKhoanNganHang = nguoiDung.TaiKhoanNganHangs?.FirstOrDefault();
            Console.WriteLine($"   - Tài khoản hiện tại: {(taiKhoanNganHang != null ? "Có" : "Không")}");

            if (hasNewBankData)
            {
                if (taiKhoanNganHang != null)
                {
                    // ✅ Cập nhật tài khoản hiện có
                    Console.WriteLine("   → Cập nhật tài khoản hiện có");
                    taiKhoanNganHang.NganHang = updateDTO.NganHang?.Trim();
                    taiKhoanNganHang.SoTaiKhoan = updateDTO.SoTaiKhoan?.Trim();
                    taiKhoanNganHang.TenChuTK = updateDTO.TenChuTK?.Trim();

                    _context.Entry(taiKhoanNganHang).State = EntityState.Modified;
                }
                else
                {
                    // ✅ Tạo mới tài khoản ngân hàng
                    Console.WriteLine("   → Tạo mới tài khoản ngân hàng");
                    var newTaiKhoan = new Data.Entities.TaiKhoanNganHang
                    {
                        MaNguoiDung = maNguoiDung,
                        NganHang = updateDTO.NganHang?.Trim(),
                        SoTaiKhoan = updateDTO.SoTaiKhoan?.Trim(),
                        TenChuTK = updateDTO.TenChuTK?.Trim()
                    };

                    await _context.TaiKhoanNganHangs.AddAsync(newTaiKhoan);
                    Console.WriteLine($"   → Đã add vào context: NH={newTaiKhoan.NganHang}, STK={newTaiKhoan.SoTaiKhoan}");
                }
            }
            else if (taiKhoanNganHang != null)
            {
                // ✅ Xóa tài khoản ngân hàng nếu không còn dữ liệu
                Console.WriteLine("   → Xóa tài khoản ngân hàng");
                _context.TaiKhoanNganHangs.Remove(taiKhoanNganHang);
            }

            try
            {
                // ✅ Lưu tất cả thay đổi
                _context.Entry(nguoiDung).State = EntityState.Modified;

                Console.WriteLine("💾 Bắt đầu SaveChanges...");
                var savedChanges = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Đã lưu {savedChanges} thay đổi vào database");

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ Lỗi DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"   InnerException: {dbEx.InnerException?.Message}");

                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"   Stack trace: {dbEx.InnerException.StackTrace}");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi Exception: {ex.Message}");
                Console.WriteLine($"   Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateProfileAsync(int maNguoiDung, UpdateProfileDTO updateDTO)
        {
            // ✅ Load người dùng kèm tài khoản ngân hàng
            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.TaiKhoanNganHangs)
                .FirstOrDefaultAsync(n => n.MaNguoiDung == maNguoiDung);

            if (nguoiDung == null) return false;

            Console.WriteLine($"✅ UpdateProfile cho user {maNguoiDung}");

            // Cập nhật thông tin cơ bản
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

            // ✅ XỬ LÝ TÀI KHOẢN NGÂN HÀNG (GIỐNG NHƯ ADMIN UPDATE)
            Console.WriteLine("🏦 Xử lý tài khoản ngân hàng...");
            Console.WriteLine($"   - NH: {updateDTO.NganHang}");
            Console.WriteLine($"   - STK: {updateDTO.SoTaiKhoan}");
            Console.WriteLine($"   - Chủ TK: {updateDTO.TenChuTK}");

            bool hasNewBankData = !string.IsNullOrWhiteSpace(updateDTO.NganHang) ||
                              !string.IsNullOrWhiteSpace(updateDTO.SoTaiKhoan) ||
                              !string.IsNullOrWhiteSpace(updateDTO.TenChuTK);

            var taiKhoanNganHang = nguoiDung.TaiKhoanNganHangs?.FirstOrDefault();
            Console.WriteLine($"   - TK hiện tại: {(taiKhoanNganHang != null ? "Có" : "Không")}");

            if (hasNewBankData)
            {
                if (taiKhoanNganHang != null)
                {
                    // Cập nhật tài khoản hiện có
                    Console.WriteLine("   → Cập nhật tài khoản hiện có");
                    taiKhoanNganHang.NganHang = updateDTO.NganHang?.Trim();
                    taiKhoanNganHang.SoTaiKhoan = updateDTO.SoTaiKhoan?.Trim();
                    taiKhoanNganHang.TenChuTK = updateDTO.TenChuTK?.Trim();
                    _context.Entry(taiKhoanNganHang).State = EntityState.Modified;
                }
                else
                {
                    // Tạo mới
                    Console.WriteLine("   → Tạo mới tài khoản ngân hàng");
                    var newTaiKhoan = new Data.Entities.TaiKhoanNganHang
                    {
                        MaNguoiDung = maNguoiDung,
                        NganHang = updateDTO.NganHang?.Trim(),
                        SoTaiKhoan = updateDTO.SoTaiKhoan?.Trim(),
                        TenChuTK = updateDTO.TenChuTK?.Trim()
                    };
                    await _context.TaiKhoanNganHangs.AddAsync(newTaiKhoan);
                }
            }
            else if (taiKhoanNganHang != null)
            {
                // Xóa nếu không còn dữ liệu
                Console.WriteLine("   → Xóa tài khoản ngân hàng");
                _context.TaiKhoanNganHangs.Remove(taiKhoanNganHang);
            }

            try
            {
                _context.Entry(nguoiDung).State = EntityState.Modified;
                Console.WriteLine("💾 Đang SaveChanges...");
                var changes = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Đã lưu {changes} thay đổi");
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"   InnerException: {dbEx.InnerException?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
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