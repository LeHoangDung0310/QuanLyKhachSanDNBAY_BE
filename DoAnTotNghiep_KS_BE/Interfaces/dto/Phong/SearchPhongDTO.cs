namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class SearchPhongDTO
    {
        public string? SoPhong { get; set; } // Tìm theo số phòng
        public int? MaLoaiPhong { get; set; } // Tìm theo mã loại phòng
        public string? TrangThai { get; set; } // Lọc theo trạng thái
        public int? MaTang { get; set; } // Lọc theo tầng
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}