namespace DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung
{
    public class NguoiDungDTO
    {
        public int MaNguoiDung { get; set; }
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChiChiTiet { get; set; }
        public int? MaPhuongXa { get; set; }
        public string? TenPhuongXa { get; set; }
        public int? MaHuyen { get; set; }
        public string? TenHuyen { get; set; }
        public int? MaTinh { get; set; }
        public string? TenTinh { get; set; }
        public string? AnhDaiDien { get; set; }
        public string TrangThai { get; set; } = "Hoạt động";
        public DateTime NgayTao { get; set; }
    }
}