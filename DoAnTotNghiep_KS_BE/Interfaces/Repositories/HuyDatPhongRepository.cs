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

        // ✅ KIỂM TRA ĐIỀU KIỆN HỦY
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

            // Kiểm tra trạng thái
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

            // Tính số ngày còn lại đến ngày nhận phòng
            var soNgayConLai = (datPhong.NgayNhanPhong.Date - DateTime.Now.Date).Days;

            // ❌ Không cho hủy nếu còn <= 7 ngày
            if (soNgayConLai <= 7)
            {
                return (false, $"Không thể hủy phòng khi còn {soNgayConLai} ngày đến ngày nhận phòng. Chỉ được hủy trước 8 ngày trở lên.", 0, 0);
            }

            // Tính tổng tiền đã thanh toán
            var daThanhToan = datPhong.ThanhToans?
                .Where(tt => tt.TrangThai == "ThanhCong")
                .Sum(tt => tt.SoTien) ?? 0;

            // Tính tổng tiền booking
            var soNgayO = (datPhong.NgayTraPhong - datPhong.NgayNhanPhong).Days;
            var tongTien = datPhong.DatPhong_Phongs?.Sum(dpp =>
                (dpp.Phong?.LoaiPhong?.GiaMoiDem ?? 0) * soNgayO) ?? 0;

            decimal phiGiu = 0;
            decimal tienHoan = 0;

            // ✅ LOGIC PHÍ HỦY
            if (soNgayConLai >= 15)
            {
                // Hủy >= 15 ngày: Miễn phí, hoàn 100%
                phiGiu = 0;
                tienHoan = daThanhToan;
            }
            else if (soNgayConLai >= 8 && soNgayConLai <= 14)
            {
                // Hủy 8-14 ngày: Phí 50%, hoàn 50%
                phiGiu = tongTien * 0.5m;
                tienHoan = daThanhToan - phiGiu;
            }

            // Đảm bảo tiền hoàn không âm
            if (tienHoan < 0) tienHoan = 0;

            return (true, $"Có thể hủy. Phí giữ: {phiGiu:N0}đ, Tiền hoàn: {tienHoan:N0}đ", phiGiu, tienHoan);
        }

        // ✅ NGƯỜI DÙNG YÊU CẦU HỦY
        public async Task<(bool success, string message, decimal? phiGiu)> YeuCauHuyDatPhongAsync(int maDatPhong, string lyDo, int maNguoiDung)
        {
            // Kiểm tra đặt phòng có thuộc về người dùng không
            var datPhong = await _context.DatPhongs
                .FirstOrDefaultAsync(dp => dp.MaDatPhong == maDatPhong && dp.MaKhachHang == maNguoiDung);

            if (datPhong == null)
            {
                return (false, "Không tìm thấy đặt phòng hoặc bạn không có quyền hủy", null);
            }

            // Kiểm tra đã có yêu cầu hủy chưa
            var daCoYeuCau = await _context.HuyDatPhongs
                .AnyAsync(h => h.MaDatPhong == maDatPhong && h.TrangThai == "ChoDuyet");

            if (daCoYeuCau)
            {
                return (false, "Đã có yêu cầu hủy đang chờ xử lý", null);
            }

            // Kiểm tra điều kiện hủy
            var (canCancel, message, phiGiu, tienHoan) = await KiemTraDieuKienHuyAsync(maDatPhong);

            if (!canCancel)
            {
                return (false, message, null);
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

        // ✅ LẤY TẤT CẢ YÊU CẦU HỦY (LỄ TÂN)
        public async Task<List<HuyDatPhongDTO>> GetAllAsync()
        {
            var list = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                .Include(h => h.NguoiDuyet)
                .OrderByDescending(h => h.NgayYeuCau)
                .ToListAsync();

            return list.Select(MapToDTO).ToList();
        }

        // ✅ LẤY CHI TIẾT YÊU CẦU HỦY
        public async Task<HuyDatPhongDTO?> GetByIdAsync(int maHuyDatPhong)
        {
            var huy = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                .Include(h => h.NguoiDuyet)
                .FirstOrDefaultAsync(h => h.MaHuyDatPhong == maHuyDatPhong);

            return huy == null ? null : MapToDTO(huy);
        }

        // ✅ LẤY DANH SÁCH YÊU CẦU HỦY CỦA KHÁCH HÀNG
        public async Task<List<HuyDatPhongDTO>> GetByKhachHangAsync(int maKhachHang)
        {
            var list = await _context.HuyDatPhongs
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.KhachHang)
                .Include(h => h.DatPhong)
                    .ThenInclude(dp => dp.DatPhong_Phongs)
                        .ThenInclude(dpp => dpp.Phong)
                .Include(h => h.NguoiDuyet)
                .Where(h => h.DatPhong.MaKhachHang == maKhachHang)
                .OrderByDescending(h => h.NgayYeuCau)
                .ToListAsync();

            return list.Select(MapToDTO).ToList();
        }

        // ✅ LỄ TÂN DUYỆT/TỪ CHỐI YÊU CẦU HỦY
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

            // Cập nhật trạng thái yêu cầu hủy
            huyDatPhong.TrangThai = choDuyet ? "DaDuyet" : "TuChoi";
            huyDatPhong.NgayXuLy = DateTime.Now;
            huyDatPhong.MaNguoiDuyet = maLeTan;
            huyDatPhong.GhiChu = ghiChu;

            if (choDuyet)
            {
                // Cập nhật trạng thái đặt phòng thành "DaHuy"
                huyDatPhong.DatPhong.TrangThai = "DaHuy";

                // Cập nhật trạng thái phòng về "Trong"
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

                // Tạo bản ghi hoàn tiền (nếu có)
                var daThanhToan = huyDatPhong.DatPhong.ThanhToans?
                    .Where(tt => tt.TrangThai == "ThanhCong")
                    .Sum(tt => tt.SoTien) ?? 0;

                var tienHoan = daThanhToan - (huyDatPhong.PhiGiu ?? 0);

                if (tienHoan > 0)
                {
                    var hoanTien = new HoanTien
                    {
                        MaHuyDatPhong = maHuyDatPhong,
                        SoTien = tienHoan,
                        NgayHoanTien = DateTime.Now,
                        TrangThai = "DangXuLy",
                        GhiChu = $"Hoàn tiền hủy đặt phòng #{huyDatPhong.MaDatPhong}"
                    };

                    _context.HoanTiens.Add(hoanTien);
                }
            }

            await _context.SaveChangesAsync();

            return (true, choDuyet ? "Đã duyệt yêu cầu hủy" : "Đã từ chối yêu cầu hủy");
        }

        // ✅ MAP TO DTO
        private HuyDatPhongDTO MapToDTO(HuyDatPhong h)
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
                GhiChu = h.GhiChu
            };
        }
    }
}