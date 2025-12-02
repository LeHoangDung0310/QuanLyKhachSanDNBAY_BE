using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Phong
{
    public class UpdatePhongDTO
    {
        [StringLength(10)]
        public string? SoPhong { get; set; }

        public int? SoGiuong { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public string? MoTa { get; set; }
        public string? TrangThai { get; set; }
        public int? MaTang { get; set; }
        public int? MaLoaiPhong { get; set; }
    }
}