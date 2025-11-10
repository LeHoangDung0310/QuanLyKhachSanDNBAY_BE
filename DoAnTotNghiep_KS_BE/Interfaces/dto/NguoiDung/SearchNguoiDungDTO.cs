namespace DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung
{
    public class SearchNguoiDungDTO
    {
        public string? SearchTerm { get; set; } // Tìm theo tên hoặc email
        public string? VaiTro { get; set; } // Lọc theo vai trò
        public string? TrangThai { get; set; } // Lọc theo trạng thái
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}