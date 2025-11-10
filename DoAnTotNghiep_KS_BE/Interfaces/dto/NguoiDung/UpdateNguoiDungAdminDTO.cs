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
        public string? DiaChi { get; set; }

        [StringLength(255)]
        [Url(ErrorMessage = "URL ảnh đại diện không hợp lệ")]
        public string? AnhDaiDien { get; set; }

        [StringLength(20)]
        [RegularExpression("^(Admin|KhachHang|NhanVien)$", ErrorMessage = "Vai trò phải là Admin, KhachHang hoặc NhanVien")]
        public string? VaiTro { get; set; }

        [StringLength(20)]
        [RegularExpression("^(Hoạt động|Tạm khóa)$", ErrorMessage = "Trạng thái phải là 'Hoạt động' hoặc 'Tạm khóa'")]
        public string? TrangThai { get; set; }
    }
}