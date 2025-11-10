using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.TienNghi
{
    public class UpdateTienNghiDTO
    {
        [StringLength(100)]
        public string? Ten { get; set; }

        [StringLength(255)]
        public string? Icon { get; set; }
    }
}