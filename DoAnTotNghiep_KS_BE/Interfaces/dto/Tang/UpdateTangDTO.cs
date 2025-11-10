using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.Tang
{
    public class UpdateTangDTO
    {
        [StringLength(50)]
        public string? TenTang { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }
    }
}