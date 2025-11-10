namespace DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi
{
    public class SearchTienNghiDTO
    {
        public string? Ten { get; set; } // Tìm theo tên tiện nghi
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}