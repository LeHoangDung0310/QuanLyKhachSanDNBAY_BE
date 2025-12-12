namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class PhongDTO
    {
        public int MaPhong { get; set; }
        public string? SoPhong { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = "Trong";
        public int? MaTang { get; set; }
        public string? TenTang { get; set; }
        public int? MaLoaiPhong { get; set; }
        public string? TenLoaiPhong { get; set; }
        public decimal? GiaMoiDem { get; set; }
        public int? SoNguoiToiDa { get; set; }
    }
}