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
    }

    public class YeuCauHuyRequest
    {
        public string LyDo { get; set; } = string.Empty;
    }

    public class DuyetHuyRequest
    {
        public bool ChoDuyet { get; set; }
        public string? GhiChu { get; set; }
    }
}