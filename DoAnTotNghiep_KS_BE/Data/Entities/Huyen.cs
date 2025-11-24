using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("Huyen")]
    public class Huyen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHuyen { get; set; }

        [Required]
        [StringLength(100)]
        public string TenHuyen { get; set; } = string.Empty;

        [Required]
        public int MaTinh { get; set; }

        // Navigation properties
        [ForeignKey("MaTinh")]
        public virtual Tinh? Tinh { get; set; }

        public virtual ICollection<PhuongXa>? PhuongXas { get; set; }
    }
}