using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong
{
    public class CreateHinhAnhPhongDTO
    {
        [Required(ErrorMessage = "Mã phòng không được để trống")]
        public int MaPhong { get; set; }

        [Required(ErrorMessage = "URL hình ảnh không được để trống")]
        [StringLength(255)]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string Url { get; set; } = string.Empty;
    }
}