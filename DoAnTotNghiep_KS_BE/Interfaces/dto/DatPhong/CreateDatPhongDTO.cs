using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong
{
    public class CreateDatPhongDTO
    {
        [Required(ErrorMessage = "Ngày nhận phòng là bắt buộc")]
        public DateTime NgayNhanPhong { get; set; }

        [Required(ErrorMessage = "Ngày trả phòng là bắt buộc")]
        public DateTime NgayTraPhong { get; set; }

        [Required(ErrorMessage = "Danh sách phòng là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 phòng")]
        public List<ChiTietPhongDatDTO> DanhSachPhong { get; set; } = new();

        public string? GhiChu { get; set; }
    }

    public class ChiTietPhongDatDTO
    {
        [Required(ErrorMessage = "Mã phòng là bắt buộc")]
        public int MaPhong { get; set; }

        [Required(ErrorMessage = "Số người là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Số người phải từ 1 đến 10")]
        public int SoNguoi { get; set; }
    }
}