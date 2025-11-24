using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("Tinh")]
    public class Tinh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTinh { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTinh { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Huyen>? Huyens { get; set; }
    }
}