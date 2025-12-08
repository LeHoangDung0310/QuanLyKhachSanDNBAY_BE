using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class DatPhongRepository : IDatPhongRepository
    {
        private readonly MyDbContext _context;

        public DatPhongRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<KiemTraThongTinDTO> KiemTraThongTinNguoiDungAsync(int maNguoiDung)
        {
            var user = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (user == null)
            {
                return new KiemTraThongTinDTO
                {
                    DayDuThongTin = false,
                    Message = "Người dùng không tồn tại"
                };
            }

            var thongTinThieu = new List<string>();

            if (string.IsNullOrWhiteSpace(user.HoTen))
                thongTinThieu.Add("Họ tên");

            if (string.IsNullOrWhiteSpace(user.SoDienThoai))
                thongTinThieu.Add("Số điện thoại");

            if (string.IsNullOrWhiteSpace(user.SoCCCD))
                thongTinThieu.Add("Số CCCD");

            if (!user.NgayCapCCCD.HasValue)
                thongTinThieu.Add("Ngày cấp CCCD");

            if (string.IsNullOrWhiteSpace(user.NoiCapCCCD))
                thongTinThieu.Add("Nơi cấp CCCD");

            if (!user.NgaySinh.HasValue)
                thongTinThieu.Add("Ngày sinh");

            if (string.IsNullOrWhiteSpace(user.GioiTinh))
                thongTinThieu.Add("Giới tính");

            if (!user.MaPhuongXa.HasValue)
                thongTinThieu.Add("Địa chỉ (Tỉnh/Huyện/Xã)");

            return new KiemTraThongTinDTO
            {
                DayDuThongTin = thongTinThieu.Count == 0,
                ThongTinThieu = thongTinThieu,
                Message = thongTinThieu.Count == 0
                    ? "Thông tin đầy đủ"
                    : $"Vui lòng cập nhật đầy đủ thông tin: {string.Join(", ", thongTinThieu)}"
            };
        }

        public async Task<List<int>> KiemTraPhongTrongAsync(List<int> danhSachMaPhong, DateTime ngayNhan, DateTime ngayTra)
        {
            var phongBiTrung = await _context.DatPhong_Phongs
                .Include(dp => dp.DatPhong)
                .Where(dp =>
                    danhSachMaPhong.Contains(dp.MaPhong) &&
                    dp.DatPhong != null &&
                    dp.DatPhong.TrangThai != "DaHuy" &&
                    dp.DatPhong.TrangThai != "HoanThanh" &&
                    (
                        (ngayNhan >= dp.DatPhong.NgayNhanPhong && ngayNhan < dp.DatPhong.NgayTraPhong) ||
                        (ngayTra > dp.DatPhong.NgayNhanPhong && ngayTra <= dp.DatPhong.NgayTraPhong) ||
                        (ngayNhan <= dp.DatPhong.NgayNhanPhong && ngayTra >= dp.DatPhong.NgayTraPhong)
                    )
                )
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToListAsync();

            return phongBiTrung;
        }

        public async Task<(bool success, string message, int? maDatPhong)> CreateDatPhongAsync(
            int maNguoiDung,
            CreateDatPhongDTO createDTO)
        {
            // 1. Kiểm tra thông tin người dùng
            var checkInfo = await KiemTraThongTinNguoiDungAsync(maNguoiDung);
            if (!checkInfo.DayDuThongTin)
            {
                return (false, checkInfo.Message!, null);
            }

            // 2. Validate ngày
            if (createDTO.NgayNhanPhong.Date < DateTime.Now.Date)
            {
                return (false, "Ngày nhận phòng phải từ hôm nay trở đi", null);
            }

            if (createDTO.NgayTraPhong.Date <= createDTO.NgayNhanPhong.Date)
            {
                return (false, "Ngày trả phòng phải sau ngày nhận phòng", null);
            }

            // 3. Kiểm tra phòng tồn tại và trạng thái
            var danhSachMaPhong = createDTO.DanhSachPhong.Select(p => p.MaPhong).ToList();
            var phongs = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => danhSachMaPhong.Contains(p.MaPhong))
                .ToListAsync();

            if (phongs.Count != danhSachMaPhong.Count)
            {
                return (false, "Một số phòng không tồn tại", null);
            }

            // Kiểm tra trạng thái phòng
            var phongKhongKhaDung = phongs.Where(p => p.TrangThai != "Trong").ToList();
            if (phongKhongKhaDung.Any())
            {
                var soPhong = string.Join(", ", phongKhongKhaDung.Select(p => p.SoPhong));
                return (false, $"Phòng {soPhong} hiện không khả dụng", null);
            }

            // 4. Kiểm tra phòng trống trong khoảng thời gian
            var phongBiTrung = await KiemTraPhongTrongAsync(
                danhSachMaPhong,
                createDTO.NgayNhanPhong,
                createDTO.NgayTraPhong
            );

            if (phongBiTrung.Any())
            {
                var soPhongTrung = string.Join(", ", phongs
                    .Where(p => phongBiTrung.Contains(p.MaPhong))
                    .Select(p => p.SoPhong));
                return (false, $"Phòng {soPhongTrung} đã được đặt trong khoảng thời gian này", null);
            }

            // 5. Kiểm tra số người
            foreach (var chiTiet in createDTO.DanhSachPhong)
            {
                var phong = phongs.First(p => p.MaPhong == chiTiet.MaPhong);
                var soNguoiToiDa = phong.LoaiPhong?.SoNguoiToiDa ?? phong.SoNguoiToiDa ?? 2;

                if (chiTiet.SoNguoi > soNguoiToiDa)
                {
                    return (false, $"Phòng {phong.SoPhong} chỉ chứa tối đa {soNguoiToiDa} người", null);
                }
            }

            // 6. Tạo đặt phòng
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var datPhong = new DatPhong
                {
                    MaKhachHang = maNguoiDung,
                    NgayDat = DateTime.Now,
                    NgayNhanPhong = createDTO.NgayNhanPhong.Date,
                    NgayTraPhong = createDTO.NgayTraPhong.Date,
                    TrangThai = "ChoDuyet",
                    MaNguoiTao = null, // THÊM: Khách tự đặt
                    LoaiDatPhong = "Online" // THÊM
                };

                _context.DatPhongs.Add(datPhong);
                await _context.SaveChangesAsync();

                // Thêm chi tiết phòng đặt
                foreach (var chiTiet in createDTO.DanhSachPhong)
                {
                    var datPhongPhong = new DatPhong_Phong
                    {
                        MaDatPhong = datPhong.MaDatPhong,
                        MaPhong = chiTiet.MaPhong,
                        SoNguoi = chiTiet.SoNguoi
                    };
                    _context.DatPhong_Phongs.Add(datPhongPhong);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Đặt phòng thành công! Vui lòng chờ xác nhận.", datPhong.MaDatPhong);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi tạo đặt phòng: {ex.Message}");
                return (false, "Có lỗi xảy ra khi đặt phòng", null);
            }
        }

        public async Task<List<DatPhongDTO>> GetDatPhongByKhachHangAsync(int maKhachHang)
        {
            var datPhongs = await _context.DatPhongs
                .Include(dp => dp.KhachHang)
                .Include(dp => dp.NguoiTao) // THÊM: Include lễ tán
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .Where(dp => dp.MaKhachHang == maKhachHang)
                .OrderByDescending(dp => dp.NgayDat)
                .ToListAsync();

            return datPhongs.Select(dp => MapToDTO(dp)).ToList();
        }

        public async Task<DatPhongDTO?> GetDatPhongByIdAsync(int maDatPhong)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.KhachHang)
                .Include(dp => dp.NguoiTao) // THÊM: Include lễ tân
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            return datPhong == null ? null : MapToDTO(datPhong);
        }

        public async Task<(bool success, string message)> HuyDatPhongAsync(int maDatPhong, int maNguoiDung)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            if (datPhong == null)
            {
                return (false, "Đặt phòng không tồn tại");
            }

            if (datPhong.MaKhachHang != maNguoiDung)
            {
                return (false, "Bạn không có quyền hủy đặt phòng này");
            }

            if (datPhong.TrangThai == "DaHuy")
            {
                return (false, "Đặt phòng đã được hủy trước đó");
            }

            if (datPhong.TrangThai == "HoanThanh")
            {
                return (false, "Không thể hủy đặt phòng đã hoàn thành");
            }

            if (datPhong.TrangThai == "DangSuDung")
            {
                return (false, "Không thể hủy đặt phòng đang sử dụng");
            }

            // Kiểm tra thời gian hủy (ví dụ: phải hủy trước 24h)
            var khoangCachNgay = (datPhong.NgayNhanPhong.Date - DateTime.Now.Date).TotalDays;
            if (khoangCachNgay < 1)
            {
                return (false, "Không thể hủy đặt phòng trong vòng 24h trước ngày nhận phòng");
            }

            datPhong.TrangThai = "DaHuy";
            await _context.SaveChangesAsync();

            return (true, "Hủy đặt phòng thành công");
        }

        public async Task<List<DatPhongDTO>> GetAllDatPhongAsync()
        {
            var datPhongs = await _context.DatPhongs
                .Include(dp => dp.KhachHang)
                .Include(dp => dp.NguoiTao) // THÊM: Include lễ tân
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                    .ThenInclude(p => p!.LoaiPhong)
                .OrderByDescending(dp => dp.NgayDat)
                .ToListAsync();

            return datPhongs.Select(dp => MapToDTO(dp)).ToList();
        }

        // Helper method
        private DatPhongDTO MapToDTO(DatPhong dp)
        {
            var soNgayO = (dp.NgayTraPhong - dp.NgayNhanPhong).Days;

            var danhSachPhong = dp.DatPhong_Phongs?.Select(dpp => new PhongDatDTO
            {
                MaPhong = dpp.MaPhong,
                SoPhong = dpp.Phong?.SoPhong,
                TenLoaiPhong = dpp.Phong?.LoaiPhong?.TenLoaiPhong,
                GiaPhong = dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0,
                SoNguoi = dpp.SoNguoi
            }).ToList() ?? new List<PhongDatDTO>();

            var tongTien = danhSachPhong.Sum(p => p.GiaPhong * soNgayO);

            return new DatPhongDTO
            {
                MaDatPhong = dp.MaDatPhong,
                MaKhachHang = dp.MaKhachHang,
                TenKhachHang = dp.KhachHang?.HoTen,
                EmailKhachHang = dp.KhachHang?.Email,
                SoDienThoai = dp.KhachHang?.SoDienThoai,
                NgayDat = dp.NgayDat,
                NgayNhanPhong = dp.NgayNhanPhong,
                NgayTraPhong = dp.NgayTraPhong,
                SoNgayO = soNgayO,
                TrangThai = dp.TrangThai,
                DanhSachPhong = danhSachPhong,
                TongTien = tongTien,

                // THÊM MỚI
                MaNguoiTao = dp.MaNguoiTao,
                TenNguoiTao = dp.NguoiTao?.HoTen, // Tên lễ tân
                LoaiDatPhong = dp.LoaiDatPhong,
                ThoiGianCheckIn = dp.ThoiGianCheckIn,
                ThoiGianCheckOut = dp.ThoiGianCheckOut
            };
        }

        // METHOD MỚI CHO ĐặT PHÒNG TRỰC TIẾP - ĐÃ SỬA
        public async Task<(bool success, string message, DatPhongTrucTiepResponseDTO? data)> CreateDatPhongTrucTiepAsync(
            int maLeTan,
            CreateDatPhongTrucTiepDTO createDTO)
        {
            // 1. Validate ngày
            if (createDTO.NgayNhanPhong.Date < DateTime.Now.Date)
            {
                return (false, "Ngày nhận phòng phải từ hôm nay trở đi", null);
            }

            if (createDTO.NgayTraPhong.Date <= createDTO.NgayNhanPhong.Date)
            {
                return (false, "Ngày trả phòng phải sau ngày nhận phòng", null);
            }

            // 2. Kiểm tra phòng tồn tại và trạng thái
            var danhSachMaPhong = createDTO.DanhSachPhong.Select(p => p.MaPhong).ToList();
            var phongs = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => danhSachMaPhong.Contains(p.MaPhong))
                .ToListAsync();

            if (phongs.Count != danhSachMaPhong.Count)
            {
                return (false, "Một số phòng không tồn tại", null);
            }

            var phongKhongKhaDung = phongs.Where(p => p.TrangThai != "Trong").ToList();
            if (phongKhongKhaDung.Any())
            {
                var soPhong = string.Join(", ", phongKhongKhaDung.Select(p => p.SoPhong));
                return (false, $"Phòng {soPhong} hiện không khả dụng", null);
            }

            // 3. Kiểm tra phòng trống trong khoảng thời gian
            var phongBiTrung = await KiemTraPhongTrongAsync(
                danhSachMaPhong,
                createDTO.NgayNhanPhong,
                createDTO.NgayTraPhong
            );

            if (phongBiTrung.Any())
            {
                var soPhongTrung = string.Join(", ", phongs
                    .Where(p => phongBiTrung.Contains(p.MaPhong))
                    .Select(p => p.SoPhong));
                return (false, $"Phòng {soPhongTrung} đã được đặt trong khoảng thời gian này", null);
            }

            // 4. Kiểm tra số người - SỬA LẠI: SoNguoiToiDa
            foreach (var chiTiet in createDTO.DanhSachPhong)
            {
                var phong = phongs.First(p => p.MaPhong == chiTiet.MaPhong);
                var soNguoiToiDa = phong.LoaiPhong?.SoNguoiToiDa ?? phong.SoNguoiToiDa ?? 2; // SỬA: ToiDa không phải ToDa

                if (chiTiet.SoNguoi > soNguoiToiDa)
                {
                    return (false, $"Phòng {phong.SoPhong} chỉ chứa tối đa {soNguoiToiDa} người", null);
                }
            }

            // 5. Tính tổng tiền
            var soNgayO = (createDTO.NgayTraPhong - createDTO.NgayNhanPhong).Days;
            var tongTien = phongs.Sum(p => (p.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO);

            // 6. Validate thanh toán
            if (createDTO.ThanhToanNgay)
            {
                if (!createDTO.SoTienThanhToan.HasValue || createDTO.SoTienThanhToan <= 0)
                {
                    return (false, "Vui lòng nhập số tiền thanh toán", null);
                }

                if (string.IsNullOrWhiteSpace(createDTO.PhuongThucThanhToan))
                {
                    return (false, "Vui lòng chọn phương thức thanh toán", null);
                }

                if (createDTO.SoTienThanhToan > tongTien)
                {
                    return (false, $"Số tiền thanh toán không được lớn hơn tổng tiền ({tongTien:N0}đ)", null);
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? maKhachHang = null;
                bool khachHangMoi = false;

                // 7. Tìm hoặc tạo khách hàng
                var existingCustomer = await _context.NguoiDungs
                    .FirstOrDefaultAsync(n =>
                        n.SoDienThoai == createDTO.SoDienThoai ||
                        (n.Email != null && createDTO.Email != null && n.Email == createDTO.Email) ||
                        n.SoCCCD == createDTO.SoCCCD);

                if (existingCustomer != null)
                {
                    // Cập nhật thông tin khách hàng cũ
                    maKhachHang = existingCustomer.MaNguoiDung;

                    existingCustomer.HoTen = createDTO.HoTen;
                    existingCustomer.SoDienThoai = createDTO.SoDienThoai;
                    existingCustomer.Email = createDTO.Email;
                    existingCustomer.SoCCCD = createDTO.SoCCCD;
                    existingCustomer.NgayCapCCCD = createDTO.NgayCapCCCD;
                    existingCustomer.NoiCapCCCD = createDTO.NoiCapCCCD;
                    existingCustomer.NgaySinh = createDTO.NgaySinh;
                    existingCustomer.GioiTinh = createDTO.GioiTinh;
                    existingCustomer.DiaChiChiTiet = createDTO.DiaChiChiTiet;
                    existingCustomer.MaPhuongXa = createDTO.MaPhuongXa;

                    _context.NguoiDungs.Update(existingCustomer);
                }
                else
                {
                    // Tạo khách hàng mới
                    khachHangMoi = true;
                    var newCustomer = new NguoiDung
                    {
                        Email = createDTO.Email ?? $"walkin_{DateTime.Now:yyyyMMddHHmmss}@hotel.local",
                        MatKhau = BCrypt.Net.BCrypt.HashPassword($"WalkIn@{createDTO.SoDienThoai}"),
                        VaiTro = "KhachHang",
                        HoTen = createDTO.HoTen,
                        SoDienThoai = createDTO.SoDienThoai,
                        SoCCCD = createDTO.SoCCCD,
                        NgayCapCCCD = createDTO.NgayCapCCCD,
                        NoiCapCCCD = createDTO.NoiCapCCCD,
                        NgaySinh = createDTO.NgaySinh,
                        GioiTinh = createDTO.GioiTinh,
                        DiaChiChiTiet = createDTO.DiaChiChiTiet,
                        MaPhuongXa = createDTO.MaPhuongXa,
                        TrangThai = "Hoạt động",
                        NgayTao = DateTime.Now
                    };

                    _context.NguoiDungs.Add(newCustomer);
                    await _context.SaveChangesAsync();
                    maKhachHang = newCustomer.MaNguoiDung;
                }

                // 8. Tạo đặt phòng
                var datPhong = new DatPhong
                {
                    MaKhachHang = maKhachHang.Value,
                    NgayDat = DateTime.Now,
                    NgayNhanPhong = createDTO.NgayNhanPhong.Date,
                    NgayTraPhong = createDTO.NgayTraPhong.Date,
                    TrangThai = createDTO.ThanhToanNgay ? "DaDuyet" : "ChoDuyet",
                    MaNguoiTao = maLeTan, // THÊM: Lưu lễ tân tạo
                    LoaiDatPhong = "TrucTiep" // THÊM
                };

                _context.DatPhongs.Add(datPhong);
                await _context.SaveChangesAsync();

                // 9. Thêm chi tiết phòng
                foreach (var chiTiet in createDTO.DanhSachPhong)
                {
                    var datPhongPhong = new DatPhong_Phong
                    {
                        MaDatPhong = datPhong.MaDatPhong,
                        MaPhong = chiTiet.MaPhong,
                        SoNguoi = chiTiet.SoNguoi
                    };
                    _context.DatPhong_Phongs.Add(datPhongPhong);
                }

                await _context.SaveChangesAsync();

                // 10. Tạo thanh toán nếu có - SỬA LẠI: Bỏ MaNguoiXuLy
                int? maThanhToan = null;
                if (createDTO.ThanhToanNgay && createDTO.SoTienThanhToan > 0)
                {
                    var thanhToan = new ThanhToan
                    {
                        MaDatPhong = datPhong.MaDatPhong,
                        SoTien = createDTO.SoTienThanhToan,
                        PhuongThuc = createDTO.PhuongThucThanhToan,
                        ThoiGian = DateTime.Now,
                        TrangThai = "ThanhCong"
                        // BỎ: MaNguoiXuLy = maLeTan (vì entity không có field này)
                    };

                    _context.ThanhToans.Add(thanhToan);
                    await _context.SaveChangesAsync();
                    maThanhToan = thanhToan.MaThanhToan;
                }

                await transaction.CommitAsync();

                var response = new DatPhongTrucTiepResponseDTO
                {
                    MaDatPhong = datPhong.MaDatPhong,
                    MaKhachHang = maKhachHang,
                    KhachHangMoi = khachHangMoi,
                    MaThanhToan = maThanhToan,
                    TongTien = tongTien,
                    SoTienDaThanhToan = createDTO.SoTienThanhToan,
                    ConLai = tongTien - (createDTO.SoTienThanhToan ?? 0)
                };

                return (true, "Đặt phòng thành công!", response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Lỗi khi tạo đặt phòng trực tiếp: {ex.Message}");
                return (false, "Có lỗi xảy ra khi đặt phòng: " + ex.Message, null);
            }
        }

        public async Task<(bool success, string message)> CheckInAsync(int maDatPhong, int maLeTan)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            if (datPhong == null)
            {
                return (false, "Đặt phòng không tồn tại");
            }

            if (datPhong.TrangThai != "DaDuyet")
            {
                return (false, $"Không thể check-in. Trạng thái hiện tại: {datPhong.TrangThai}");
            }

            if (datPhong.NgayNhanPhong.Date > DateTime.Now.Date)
            {
                return (false, $"Chưa đến ngày nhận phòng ({datPhong.NgayNhanPhong:dd/MM/yyyy})");
            }

            // ✅ Cập nhật trạng thái và THỜI GIAN CHECK-IN
            datPhong.TrangThai = "DangSuDung";
            datPhong.ThoiGianCheckIn = DateTime.Now; // ✅ LƯU THỜI GIAN

            // Cập nhật trạng thái phòng
            if (datPhong.DatPhong_Phongs != null)
            {
                foreach (var dpp in datPhong.DatPhong_Phongs)
                {
                    if (dpp.Phong != null)
                    {
                        dpp.Phong.TrangThai = "DangSuDung";
                    }
                }
            }

            await _context.SaveChangesAsync();

            return (true, "Check-in thành công");
        }

        public async Task<(bool success, string message)> CheckOutAsync(int maDatPhong, int maLeTan)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                .Include(dp => dp.ThanhToans)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            if (datPhong == null)
            {
                return (false, "Đặt phòng không tồn tại");
            }

            if (datPhong.TrangThai != "DangSuDung")
            {
                return (false, $"Không thể check-out. Trạng thái hiện tại: {datPhong.TrangThai}");
            }

            // Kiểm tra thanh toán đầy đủ
            var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
            var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            var daThanhToan = datPhong.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien) ?? 0;

            if (daThanhToan < tongTien)
            {
                var conLai = tongTien - daThanhToan;
                return (false, $"Chưa thanh toán đủ. Còn thiếu: {conLai:N0}đ");
            }

            // ✅ Cập nhật trạng thái và THỜI GIAN CHECK-OUT
            datPhong.TrangThai = "HoanThanh";
            datPhong.ThoiGianCheckOut = DateTime.Now; // ✅ LƯU THỜI GIAN

            // Cập nhật trạng thái phòng về "Trong"
            if (datPhong.DatPhong_Phongs != null)
            {
                foreach (var dpp in datPhong.DatPhong_Phongs)
                {
                    if (dpp.Phong != null)
                    {
                        dpp.Phong.TrangThai = "Trong";
                    }
                }
            }

            await _context.SaveChangesAsync();

            return (true, "Check-out thành công");
        }
    }
}