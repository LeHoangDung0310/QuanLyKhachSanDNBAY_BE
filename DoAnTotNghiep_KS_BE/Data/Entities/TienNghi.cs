using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("TienNghi")]
    public class TienNghi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTienNghi { get; set; }

        [StringLength(100)]
        public string? Ten { get; set; }

        [StringLength(255)]
        public string? Icon { get; set; }

        // Navigation properties
        public virtual ICollection<Phong_TienNghi>? Phong_TienNghis { get; set; }
    }
}