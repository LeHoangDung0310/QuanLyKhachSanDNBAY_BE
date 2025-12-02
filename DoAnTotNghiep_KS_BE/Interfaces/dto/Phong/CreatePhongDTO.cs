using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class CreatePhongDTO
    {
        [Required(ErrorMessage = "Số phòng không được để trống")]
        [StringLength(10)]
        public string SoPhong { get; set; } = string.Empty;

        public int? SoGiuong { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public string? MoTa { get; set; }
        public int? MaTang { get; set; }
        public int? MaLoaiPhong { get; set; }
    }
}