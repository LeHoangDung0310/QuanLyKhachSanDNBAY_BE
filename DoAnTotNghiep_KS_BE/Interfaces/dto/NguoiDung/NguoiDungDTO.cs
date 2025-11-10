namespace DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung
{
    public class NguoiDungDTO
    {
        public int MaNguoiDung { get; set; }
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? AnhDaiDien { get; set; }
        public string TrangThai { get; set; } = "Hoạt động";
        public DateTime NgayTao { get; set; }
    }
}