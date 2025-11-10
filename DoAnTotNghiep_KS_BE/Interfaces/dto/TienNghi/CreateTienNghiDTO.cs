using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi
{
    public class CreateTienNghiDTO
    {
        [Required(ErrorMessage = "Tên tiện nghi không được để trống")]
        [StringLength(100)]
        public string Ten { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Icon { get; set; }
    }
}