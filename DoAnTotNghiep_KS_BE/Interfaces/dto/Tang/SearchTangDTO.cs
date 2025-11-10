namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Tang
{
    public class SearchTangDTO
    {
        public string? TenTang { get; set; } // Tìm theo tên tầng
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}