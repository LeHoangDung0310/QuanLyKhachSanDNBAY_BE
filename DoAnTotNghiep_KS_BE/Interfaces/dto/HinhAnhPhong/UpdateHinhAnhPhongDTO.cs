using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.HinhAnhPhong
{
    public class UpdateHinhAnhPhongDTO
    {
        [StringLength(255)]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string? Url { get; set; }
    }
}