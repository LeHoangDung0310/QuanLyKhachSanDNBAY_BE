namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong
{
    public class HinhAnhLPhongDTO
    {
        public int MaHinhAnh { get; set; }
        public int MaLoaiPhong { get; set; }
        public string? TenLoaiPhong { get; set; }
        public string Url { get; set; } = null!;
    }
}