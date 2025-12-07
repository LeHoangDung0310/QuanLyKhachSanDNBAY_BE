namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class PhongTrongDTO
    {
        public int MaPhong { get; set; }
        public string? SoPhong { get; set; }
        public string TrangThai { get; set; } = "Trong";

        // Thông tin loại phòng
        public string? TenLoaiPhong { get; set; }
        public decimal? GiaMoiDem { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public decimal? DienTich { get; set; }
        public string? MoTa { get; set; }

        // Thông tin tầng
        public string? TenTang { get; set; }
    }
}