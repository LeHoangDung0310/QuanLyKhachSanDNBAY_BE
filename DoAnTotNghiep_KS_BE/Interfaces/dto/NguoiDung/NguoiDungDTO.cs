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

        // Thông tin bổ sung
        public string? SoCCCD { get; set; }
        public DateTime? NgayCapCCCD { get; set; }
        public string? NoiCapCCCD { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }

        // Thông tin tài khoản ngân hàng
        public string? NganHang { get; set; }
        public string? SoTaiKhoan { get; set; }
        public string? TenChuTK { get; set; }
    }
}