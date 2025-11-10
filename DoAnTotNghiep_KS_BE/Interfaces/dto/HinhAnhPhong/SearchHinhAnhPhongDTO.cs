namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong
{
    public class SearchHinhAnhPhongDTO
    {
        public int? MaPhong { get; set; } // Lọc theo phòng
        public string? SoPhong { get; set; } // Tìm theo số phòng
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}