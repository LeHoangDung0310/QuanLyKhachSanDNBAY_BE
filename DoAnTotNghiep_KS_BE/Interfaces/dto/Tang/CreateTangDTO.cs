using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Tang
{
    public class CreateTangDTO
    {
        [Required(ErrorMessage = "Tên tầng không được để trống")]
        [StringLength(50)]
        public string TenTang { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }
    }
}