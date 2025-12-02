namespace DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong
{
    public class SearchLoaiPhongDTO
    {
        public string? TenLoaiPhong { get; set; }
        public int? SoNguoiToiDaMin { get; set; }
        public int? SoNguoiToiDaMax { get; set; }
        public int? SoGiuongMin { get; set; }
        public int? SoGiuongMax { get; set; }
        public decimal? GiaMin { get; set; }
        public decimal? GiaMax { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}