namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HuyDatPhong
{
    public class HuyDatPhongDTO
    {
        public int MaHuyDatPhong { get; set; }
        public int MaDatPhong { get; set; }
        public string? TenKhachHang { get; set; }
        public string? EmailKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime? NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public decimal? PhiGiu { get; set; }
        public decimal TienHoan { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public string? TenNguoiDuyet { get; set; }
        public string? GhiChu { get; set; }

        // ✅ THÊM MỚI - Thông tin tài khoản ngân hàng
        public int? MaTaiKhoan { get; set; }
        public string? NganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTK { get; set; }

        // ✅ Thông tin hoàn tiền
        public int? MaHoanTien { get; set; }
        public string? TrangThaiHoanTien { get; set; }
        public DateTime? NgayHoanTien { get; set; }
        public string? TenQuanTriHoanTien { get; set; }
    }

    public class YeuCauHuyRequest
    {
        public string LyDo { get; set; } = string.Empty;

        // ✅ THÊM MỚI - Thông tin ngân hàng (bắt buộc nếu chưa có)
        public string? NganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTK { get; set; }
    }

    public class DuyetHuyRequest
    {
        public bool ChoDuyet { get; set; }
        public string? GhiChu { get; set; }
    }

    // ✅ THÊM MỚI - Request cho Admin hoàn tiền
    public class XacNhanHoanTienRequest
    {
        public string? GhiChu { get; set; }
    }
}