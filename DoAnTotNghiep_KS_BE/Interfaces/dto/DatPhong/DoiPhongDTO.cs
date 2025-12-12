using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong
{
    public class DoiPhongRequestDTO
    {
        [Required(ErrorMessage = "Mã phòng cũ là bắt buộc")]
        public int MaPhongCu { get; set; }

        [Required(ErrorMessage = "Mã phòng mới là bắt buộc")]
        public int MaPhongMoi { get; set; }

        public string? LyDo { get; set; }
    }

    public class DoiPhongResponseDTO
    {
        public int MaDatPhong { get; set; }
        public int MaPhongCu { get; set; }
        public string? SoPhongCu { get; set; }
        public int MaPhongMoi { get; set; }
        public string? SoPhongMoi { get; set; }
        public bool CungLoaiPhong { get; set; }
        public decimal GiaPhongCu { get; set; }
        public decimal GiaPhongMoi { get; set; }
        public int SoNgayConLai { get; set; }
        public decimal PhiChenhLech { get; set; }
        public string? ThongBao { get; set; }
        public DateTime ThoiGianDoiPhong { get; set; }
    }
}
