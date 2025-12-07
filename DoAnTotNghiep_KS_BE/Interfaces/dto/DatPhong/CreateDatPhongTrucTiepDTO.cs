using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong
{
    // DTO cho lễ tân tạo đặt phòng trực tiếp
    public class CreateDatPhongTrucTiepDTO
    {
        // Thông tin khách hàng
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số CCCD là bắt buộc")]
        [StringLength(20, ErrorMessage = "Số CCCD không quá 20 ký tự")]
        public string SoCCCD { get; set; } = string.Empty;

        public DateTime? NgayCapCCCD { get; set; }
        public string? NoiCapCCCD { get; set; }

        public DateTime? NgaySinh { get; set; }

        [RegularExpression("^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính phải là Nam, Nữ hoặc Khác")]
        public string? GioiTinh { get; set; }

        public string? DiaChiChiTiet { get; set; }
        public int? MaPhuongXa { get; set; }

        // Thông tin đặt phòng
        [Required(ErrorMessage = "Ngày nhận phòng là bắt buộc")]
        public DateTime NgayNhanPhong { get; set; }

        [Required(ErrorMessage = "Ngày trả phòng là bắt buộc")]
        public DateTime NgayTraPhong { get; set; }

        [Required(ErrorMessage = "Danh sách phòng là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 phòng")]
        public List<ChiTietPhongDatDTO> DanhSachPhong { get; set; } = new();

        public string? GhiChu { get; set; }

        // Thông tin thanh toán (nếu thanh toán ngay)
        public bool ThanhToanNgay { get; set; } = false;

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải >= 0")]
        public decimal? SoTienThanhToan { get; set; }

        [RegularExpression("^(TienMat|ChuyenKhoan|TheATM)$", ErrorMessage = "Phương thức thanh toán không hợp lệ")]
        public string? PhuongThucThanhToan { get; set; }
    }

    // Response khi tạo đặt phòng trực tiếp
    public class DatPhongTrucTiepResponseDTO
    {
        public int MaDatPhong { get; set; }
        public int? MaKhachHang { get; set; }
        public bool KhachHangMoi { get; set; }
        public int? MaThanhToan { get; set; }
        public decimal TongTien { get; set; }
        public decimal? SoTienDaThanhToan { get; set; }
        public decimal? ConLai { get; set; }
    }
}