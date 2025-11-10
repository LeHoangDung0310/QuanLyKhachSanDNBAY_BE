namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class PhongDTO
    {
        public int MaPhong { get; set; }
        public string? SoPhong { get; set; }
        public string? TenLoai { get; set; }
        public int? DienTich { get; set; }
        public int? SoGiuong { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public string? HuongNhin { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaMoiDem { get; set; }
        public string TrangThai { get; set; } = "Trong";
        public int? MaTang { get; set; }
        public string? TenTang { get; set; }
    }
}