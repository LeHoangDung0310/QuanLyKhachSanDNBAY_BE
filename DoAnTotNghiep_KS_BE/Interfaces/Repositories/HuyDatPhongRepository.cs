using DoAnTotNghiep_KS_BE.Data;
using DoAnTotNghiep_KS_BE.Data.Entities;
using DoAnTotNghiep_KS_BE.Interfaces.dto.HuyDatPhong;
using DoAnTotNghiep_KS_BE.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep_KS_BE.Interfaces.Repositories
{
    public class HuyDatPhongRepository : IHuyDatPhongRepository
    {
        private readonly MyDbContext _context;

        public HuyDatPhongRepository(MyDbContext context)
        {
            _context = context;
        }

        // ✅ KIỂM TRA ĐIỀU KIỆN HỦY (giữ nguyên)
        public async Task<(bool canCancel, string message, decimal phiGiu, decimal tienHoan)> KiemTraDieuKienHuyAsync(int maDatPhong)
        {
            var datPhong = await _context.DatPhongs
                .Include(dp => dp.DatPhong_Phongs)
                    .ThenInclude(dpp => dpp.Phong)
                        .ThenInclude(p => p.LoaiPhong)
                .Include(dp => dp.ThanhToans)
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong);

            if (datPhong == null)
            {
                return (false, "Đặt phòng không tồn tại", 0, 0);
            }

            if (datPhong.TrangThai == "DaHuy")
            {
                return (false, "Đặt phòng đã bị hủy trước đó", 0, 0);
            }

            if (datPhong.TrangThai == "HoanThanh")
            {
                return (false, "Không thể hủy đặt phòng đã hoàn thành", 0, 0);
            }

            if (datPhong.TrangThai == "DangSuDung")
            {
                return (false, "Không thể hủy khi đang sử dụng phòng", 0, 0);
            }

            var soNgayConLai = (datPhong.NgayNhanPhong.Date - DateTime.Now.Date).Days;

            if (soNgayConLai <= 7)
            {
                return (false, $"Không thể hủy phòng khi còn {soNgayConLai} ngày đến ngày nhận phòng. Chỉ được hủy trước 8 ngày trở lên.", 0, 0);
            }

            var daThanhToan = datPhong.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien) ?? 0;

            var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
            var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            decimal phiGiu = 0;
            decimal tienHoan = 0;

            if (soNgayConLai >= 15)
            {
                phiGiu = 0;
                tienHoan = daThanhToan;
            }
            else if (soNgayConLai >= 8 && soNgayConLai <= 14)
            {
                phiGiu = tongTien * 0.5m;
                tienHoan = daThanhToan - phiGiu;
            }

            if (tienHoan < 0) tienHoan = 0;

            return (true, $"Có thể hủy. Phí giữ: {phiGiu:N0}đ, Tiền hoàn: {tienHoan:N0}đ", phiGiu, tienHoan);
        }

        // ✅ NGƯỜI DÙNG YÊU CẦU HỦY (CẬP NHẬT - kèm thông tin ngân hàng)
        public async Task<(bool success, string message, decimal? phiGiu)> YeuCauHuyDatPhongAsync(
            int maDatPhong,
            string lyDo,
            int maNguoiDung,
            string? nganHang,
            string? soTaiKhoan,
            string? tenChuTK)
        {
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong && dp.MaKhachHang == maNguoiDung);

            if (datPhong == null)
            {
                return (false, "Không tìm thấy đặt phòng hoặc bạn không có quyền hủy", null);
            }

            var daCoYeuCau = await _context.HuyDatPhongs
                .AnyAsync(h => h.MaDatPhong == maDatPhong && h.TrangThai == "ChoDuyet");

            if (daCoYeuCau)
            {
                return (false, "Đã có yêu cầu hủy đang chờ xử lý", null);
            }

            var (canCancel, message, phiGiu, tienHoan) = await KiemTraDieuKienHuyAsync(maDatPhong);

            if (!canCancel)
            {
                return (false, message, null);
            }

            // ✅ KIỂM TRA/TẠO THÔNG TIN NGÂN HÀNG
            var taiKhoanNH = await _context.TaiKhoanNganHangs
                .FirstOrDefaultAsync(tk => tk.MaNguoiDung == maNguoiDung);

            // Nếu chưa có tài khoản và người dùng không cung cấp thông tin
            if (taiKhoanNH == null && (string.IsNullOrEmpty(nganHang) || string.IsNullOrEmpty(soTaiKhoan) || string.IsNullOrEmpty(tenChuTK)))
            {
                return (false, "Vui lòng cung cấp thông tin tài khoản ngân hàng để nhận hoàn tiền", null);
            }

            // Nếu chưa có tài khoản và người dùng cung cấp thông tin → Tạo mới
            if (taiKhoanNH == null && !string.IsNullOrEmpty(nganHang))
            {
                taiKhoanNH = new TaiKhoanNganHang
                {
                    MaNguoiDung = maNguoiDung,
                    NganHang = nganHang,
                    SoTaiKhoan = soTaiKhoan,
                    TenChuTK = tenChuTK
                };
                _context.TaiKhoanNganHangs.Add(taiKhoanNH);
                await _context.SaveChangesAsync();
            }
            // Nếu đã có nhưng người dùng muốn cập nhật
            else if (taiKhoanNH != null && !string.IsNullOrEmpty(nganHang))
            {
                taiKhoanNH.NganHang = nganHang;
                taiKhoanNH.SoTaiKhoan = soTaiKhoan;
                taiKhoanNH.TenChuTK = tenChuTK;
                await _context.SaveChangesAsync();
            }

            // Tạo yêu cầu hủy
            var huyDatPhong = new HuyDatPhong
            {
                MaDatPhong = maDatPhong,
                NgayYeuCau = DateTime.Now,
                LyDo = lyDo,
                TrangThai = "ChoDuyet",
                PhiGiu = phiGiu
            };

            _context.HuyDatPhongs.Add(huyDatPhong);
            await _context.SaveChangesAsync();

            return (true, $"Đã gửi yêu cầu hủy thành công. {message}", phiGiu);
        }

        // ✅ LỄ TÂN DUYỆT (CẬP NHẬT - tạo bản ghi hoàn tiền)
        public async Task<(bool success, string message)> DuyetHuyDatPhongAsync(int maHuyDatPhong, bool choDuyet, int maLeTan, string? ghiChu)
        {
            var huyDatPhong = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.ThanhToans)
                .FirstOrDefaultAsync(h => h.MaHuyDatPhong == maHuyDatPhong);

            if (huyDatPhong == null)
            {
                return (false, "Không tìm thấy yêu cầu hủy");
            }

            if (huyDatPhong.TrangThai != "ChoDuyet")
            {
                return (false, $"Yêu cầu đã được xử lý trước đó (Trạng thái: {huyDatPhong.TrangThai})");
            }

            huyDatPhong.TrangThai = choDuyet ? "DaDuyet" : "TuChoi";
            huyDatPhong.NgayXuLy = DateTime.Now;
            huyDatPhong.MaNguoiDuyet = maLeTan;
            huyDatPhong.GhiChu = ghiChu;

            if (choDuyet)
            {
                // Cập nhật trạng thái đặt phòng
                huyDatPhong.DatPhong.TrangThai = "DaHuy";

                // Cập nhật trạng thái phòng
                var datPhong_Phongs = await _context.DatPhong_Phongs
                    .Include(dpp => dpp.Phong)
                    .Where(dpp => dpp.MaDatPhong == huyDatPhong.MaDatPhong)
                    .ToListAsync();

                foreach (var dpp in datPhong_Phongs)
                {
                    if (dpp.Phong != null)
                    {
                        dpp.Phong.TrangThai = "Trong";
                    }
                }

                // ✅ TẠO BẢN GHI HOÀN TIỀN (ChoXuLy - chờ Admin xử lý)
                var daThanhToan = huyDatPhong.DatPhong.ThanhToans?
                    .Where(tt => tt.TrangThai == "ThanhCong")
                    .Sum(tt => tt.SoTien) ?? 0;

                var tienHoan = daThanhToan - (huyDatPhong.PhiGiu ?? 0);

                if (tienHoan > 0)
                {
                    var hoanTien = new HoanTien
                    {
                        MaHuyDatPhong = maHuyDatPhong,
                        TrangThai = "ChoXuLy", // ✅ Chờ Admin xử lý
                        MaQuanTri = null,
                        NgayXuLy = null
                    };

                    _context.HoanTiens.Add(hoanTien);
                }
            }

            await _context.SaveChangesAsync();

            return (true, choDuyet ? "Đã duyệt yêu cầu hủy. Chờ Admin hoàn tiền" : "Đã từ chối yêu cầu hủy");
        }

        // ✅ ADMIN LẤY DANH SÁCH CHỜ HOÀN TIỀN
        public async Task<List<HuyDatPhongDTO>> GetDanhSachChoHoanTienAsync()
        {
            var list = await _context.HoanTiens
                .Include(ht => ht.HuyDatPhong)
                    .ThenInclude(h => h.DatPhong)
                        .ThenInclude(dp => dp.KhachHang)
                            .ThenInclude(kh => kh.TaiKhoanNganHangs)
                .Include(ht => ht.HuyDatPhong)
                    .ThenInclude(h => h.DatPhong)
                        .ThenInclude(dp => dp.DatPhong_Phongs)
                            .ThenInclude(dpp => dpp.Phong)
                                .ThenInclude(p => p.LoaiPhong)
                .Include(ht => ht.QuanTri)
                .Where(ht => ht.TrangThai == "ChoXuLy")
                .OrderBy(ht => ht.MaHoanTien)
                .ToListAsync();

            return list.Select(ht => MapToDTO(ht.HuyDatPhong, ht)).ToList();
        }

        // ✅ ADMIN XÁC NHẬN ĐÃ HOÀN TIỀN
        public async Task<(bool success, string message)> XacNhanHoanTienAsync(int maHoanTien, int maQuanTri, string? ghiChu)
        {
            var hoanTien = await _context.HoanTiens
                .Include(ht => ht.HuyDatPhong)
                .FirstOrDefaultAsync(ht => ht.MaHoanTien == maHoanTien);

            if (hoanTien == null)
            {
                return (false, "Không tìm thấy yêu cầu hoàn tiền");
            }

            if (hoanTien.TrangThai != "ChoXuLy")
            {
                return (false, $"Yêu cầu đã được xử lý (Trạng thái: {hoanTien.TrangThai})");
            }

            // ✅ CẬP NHẬT TRẠNG THÁI HOÀN TIỀN
            hoanTien.TrangThai = "DaHoan";
            hoanTien.MaQuanTri = maQuanTri;
            hoanTien.NgayXuLy = DateTime.Now;

            // Cập nhật ghi chú vào yêu cầu hủy
            if (!string.IsNullOrEmpty(ghiChu) && hoanTien.HuyDatPhong != null)
            {
                hoanTien.HuyDatPhong.GhiChu = (hoanTien.HuyDatPhong.GhiChu ?? "") + $"\n[Admin] {ghiChu}";
            }

            await _context.SaveChangesAsync();

            return (true, "Đã xác nhận hoàn tiền thành công");
        }

        // ✅ LẤY TẤT CẢ (giữ nguyên)
        public async Task<List<HuyDatPhongDTO>> GetAllAsync()
        {
            var list = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                        .ThenInclude(kh => kh.TaiKhoanNganHangs)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                            .ThenInclude(p => p.LoaiPhong)
                .Include(h => h.NguoiDuyet)
                .Include(h => h.HoanTien)
                    .ThenInclude(ht => ht.QuanTri)
                .OrderByDescending(h => h.NgayYeuCau)
                .ToListAsync();

            return list.Select(h => MapToDTO(h, h.HoanTien)).ToList();
        }

        // ✅ LẤY CHI TIẾT (giữ nguyên)
        public async Task<HuyDatPhongDTO?> GetByIdAsync(int maHuyDatPhong)
        {
            var huy = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                        .ThenInclude(kh => kh.TaiKhoanNganHangs)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                            .ThenInclude(p => p.LoaiPhong)
                .Include(h => h.NguoiDuyet)
                .Include(h => h.HoanTien)
                    .ThenInclude(ht => ht.QuanTri)
                .FirstOrDefaultAsync(h => h.MaHuyDatPhong == maHuyDatPhong);

            return huy == null ? null : MapToDTO(huy, huy.HoanTien);
        }

        // ✅ LẤY THEO KHÁCH HÀNG (giữ nguyên)
        public async Task<List<HuyDatPhongDTO>> GetByKhachHangAsync(int maKhachHang)
        {
            var list = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                        .ThenInclude(kh => kh.TaiKhoanNganHangs)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                            .ThenInclude(p => p.LoaiPhong)
                .Include(h => h.NguoiDuyet)
                .Include(h => h.HoanTien)
                    .ThenInclude(ht => ht.QuanTri)
                .Where(h => h.DatPhong.MaKhachHang == maKhachHang)
                .OrderByDescending(h => h.NgayYeuCau)
                .ToListAsync();

            return list.Select(h => MapToDTO(h, h.HoanTien)).ToList();
        }

        // ✅ MAP TO DTO (CẬP NHẬT - thêm thông tin ngân hàng và hoàn tiền)
        private HuyDatPhongDTO MapToDTO(HuyDatPhong h, HoanTien? hoanTien)
        {
            var soNgayO = h.DatPhong != null
                ? (h.DatPhong.NgayTraPhong - h.DatPhong.NgayNhanPhong).Days
                : 0;

            var tongTien = h.DatPhong?.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            var daThanhToan = h.DatPhong?.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien) ?? 0;

            var tienHoan = daThanhToan - (h.PhiGiu ?? 0);

            // ✅ Lấy thông tin tài khoản ngân hàng
            var taiKhoanNH = h.DatPhong?.KhachHang?.TaiKhoanNganHangs?.FirstOrDefault();

            return new HuyDatPhongDTO
            {
                MaHuyDatPhong = h.MaHuyDatPhong,
                MaDatPhong = h.MaDatPhong,
                TenKhachHang = h.DatPhong?.KhachHang?.HoTen,
                EmailKhachHang = h.DatPhong?.KhachHang?.Email,
                SoDienThoai = h.DatPhong?.KhachHang?.SoDienThoai,
                NgayNhanPhong = h.DatPhong?.NgayNhanPhong,
                NgayTraPhong = h.DatPhong?.NgayTraPhong,
                TongTien = tongTien,
                DaThanhToan = daThanhToan,
                NgayYeuCau = h.NgayYeuCau,
                LyDo = h.LyDo,
                TrangThai = h.TrangThai,
                PhiGiu = h.PhiGiu,
                TienHoan = tienHoan > 0 ? tienHoan : 0,
                NgayXuLy = h.NgayXuLy,
                TenNguoiDuyet = h.NguoiDuyet?.HoTen,
                GhiChu = h.GhiChu,

                // ✅ Thông tin ngân hàng
                MaTaiKhoan = taiKhoanNH?.MaTaiKhoan,
                NganHang = taiKhoanNH?.NganHang,
                SoTaiKhoan = taiKhoanNH?.SoTaiKhoan,
                TenChuTK = taiKhoanNH?.TenChuTK,

                // ✅ Thông tin hoàn tiền
                MaHoanTien = hoanTien?.MaHoanTien,
                TrangThaiHoanTien = hoanTien?.TrangThai,
                NgayHoanTien = hoanTien?.NgayXuLy,
                TenQuanTriHoanTien = hoanTien?.QuanTri?.HoTen
            };
        }
    }
}