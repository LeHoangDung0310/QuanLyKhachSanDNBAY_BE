using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class CreatePhongDTO
    {
        [Required(ErrorMessage = "Số phòng không được để trống")]
        [StringLength(10)]
        public string SoPhong { get; set; } = string.Empty;

        [StringLength(100)]
        public string? TenLoai { get; set; }

        [Range(1, 1000, ErrorMessage = "Diện tích phải từ 1-1000 m²")]
        public int? DienTich { get; set; }

        [Range(1, 20, ErrorMessage = "Số giường phải từ 1-20")]
        public int? SoGiuong { get; set; }

        [Range(1, 100, ErrorMessage = "Số người tối đa phải từ 1-100")]
        public int? SoNguoiToiDa { get; set; }

        [StringLength(100)]
        public string? HuongNhin { get; set; }

        public string? MoTa { get; set; }

        [Range(0, 999999999, ErrorMessage = "Giá phải >= 0")]
        public decimal? GiaMoiDem { get; set; }

        public int? MaTang { get; set; }
    }
}