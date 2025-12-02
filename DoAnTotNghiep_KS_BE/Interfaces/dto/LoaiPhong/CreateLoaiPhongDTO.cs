using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.LoaiPhong
{
    public class CreateLoaiPhongDTO
    {
        [Required(ErrorMessage = "Tên loại phòng không được để trống")]
        [StringLength(100)]
        public string TenLoaiPhong { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public int? SoNguoiToiDa { get; set; }
        public int? SoGiuong { get; set; }
        public int? DienTich { get; set; }
        public decimal? GiaMoiDem { get; set; }
    }
}