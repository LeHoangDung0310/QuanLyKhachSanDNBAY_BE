using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.NguoiDung
{
    public class UpdateNguoiDungAdminDTO
    {
        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(15)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [StringLength(255)]
        public string? DiaChiChiTiet { get; set; }

        public int? MaPhuongXa { get; set; }

        [StringLength(20)]
        [RegularExpression("(Admin|KhachHang|LeTan)", ErrorMessage = "Vai trò phải là Admin, KhachHang hoặc LeTan")]
        public string? VaiTro { get; set; }

        [StringLength(20)]
        [RegularExpression("^(Hoạt động|Tạm khóa)$", ErrorMessage = "Trạng thái phải là 'Hoạt động' hoặc 'Tạm khóa'")]
        public string? TrangThai { get; set; }

        // Thông tin CCCD
        [StringLength(20)]
        public string? SoCCCD { get; set; }

        public DateTime? NgayCapCCCD { get; set; }

        [StringLength(200)]
        public string? NoiCapCCCD { get; set; }

        // Thông tin cá nhân
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [RegularExpression("^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính phải là Nam, Nữ hoặc Khác")]
        public string? GioiTinh { get; set; }
    }
}