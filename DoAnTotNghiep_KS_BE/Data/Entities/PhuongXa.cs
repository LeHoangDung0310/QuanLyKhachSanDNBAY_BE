using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("PhuongXa")]
    public class PhuongXa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhuongXa { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPhuongXa { get; set; } = string.Empty;

        [Required]
        public int MaHuyen { get; set; }

        // Navigation properties
        [ForeignKey("MaHuyen")]
        public virtual Huyen? Huyen { get; set; }

        public virtual ICollection<NguoiDung>? NguoiDungs { get; set; }
    }
}