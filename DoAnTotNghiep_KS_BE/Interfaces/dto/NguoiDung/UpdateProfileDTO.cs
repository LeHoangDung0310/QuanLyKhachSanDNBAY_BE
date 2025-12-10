using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung
{
    public class UpdateProfileDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [StringLength(255)]
        public string? DiaChiChiTiet { get; set; }

        public int? MaPhuongXa { get; set; }

        // Thông tin CCCD
        [StringLength(20, ErrorMessage = "Số CCCD không quá 20 ký tự")]
        public string? SoCCCD { get; set; }

        public DateTime? NgayCapCCCD { get; set; }

        [StringLength(200, ErrorMessage = "Nơi cấp CCCD không quá 200 ký tự")]
        public string? NoiCapCCCD { get; set; }

        // Thông tin cá nhân
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [RegularExpression("^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính phải là Nam, Nữ hoặc Khác")]
        public string? GioiTinh { get; set; }
        // ✅ THÊM THÔNG TIN NGÂN HÀNG
        [StringLength(100, ErrorMessage = "Tên ngân hàng không quá 100 ký tự")]
        public string? NganHang { get; set; }

        [StringLength(50, ErrorMessage = "Số tài khoản không quá 50 ký tự")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số tài khoản chỉ chứa chữ số")]
        public string? SoTaiKhoan { get; set; }

        [StringLength(100, ErrorMessage = "Tên chủ tài khoản không quá 100 ký tự")]
        [RegularExpression(@"^[A-Z\s]+$", ErrorMessage = "Tên chủ tài khoản phải viết hoa không dấu (VD: NGUYEN VAN A)")]
        public string? TenChuTK { get; set; }

    }
}