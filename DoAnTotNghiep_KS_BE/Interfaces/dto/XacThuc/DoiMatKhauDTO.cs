using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.XacThuc
{
    public class DoiMatKhauDTO
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string MatKhauHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }
}