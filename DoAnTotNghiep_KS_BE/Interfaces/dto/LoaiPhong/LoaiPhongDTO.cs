namespace DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong
{
    public class LoaiPhongDTO
    {
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public int? SoGiuong { get; set; }
        public int? DienTich { get; set; }
        public decimal? GiaMoiDem { get; set; }
    }
}