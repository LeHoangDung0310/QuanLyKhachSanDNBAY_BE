using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class UpdatePhongDTO
    {
        [StringLength(10)]
        public string? SoPhong { get; set; }

        [StringLength(100)]
        public string? TenLoai { get; set; }

        [Range(1, 1000)]
        public int? DienTich { get; set; }

        [Range(1, 20)]
        public int? SoGiuong { get; set; }

        [Range(1, 100)]
        public int? SoNguoiToiDa { get; set; }

        [StringLength(100)]
        public string? HuongNhin { get; set; }

        public string? MoTa { get; set; }

        [Range(0, 999999999)]
        public decimal? GiaMoiDem { get; set; }

        [StringLength(20)]
        public string? TrangThai { get; set; }

        public int? MaTang { get; set; }
    }
}