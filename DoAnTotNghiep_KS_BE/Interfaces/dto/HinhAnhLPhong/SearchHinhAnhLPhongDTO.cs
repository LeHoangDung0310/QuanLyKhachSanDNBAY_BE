namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhLPhong
{
    public class SearchHinhAnhLPhongDTO
    {
        public int? MaLoaiPhong { get; set; }
        public string? TenLoaiPhong { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}