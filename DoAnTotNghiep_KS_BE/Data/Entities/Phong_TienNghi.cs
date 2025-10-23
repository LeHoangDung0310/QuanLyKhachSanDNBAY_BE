using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("Phong_TienNghi")]
    public class Phong_TienNghi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhongTienNghi { get; set; }

        [Required]
        [ForeignKey("Phong")]
        public int MaPhong { get; set; }

        [Required]
        [ForeignKey("TienNghi")]
        public int MaTienNghi { get; set; }

        // Navigation properties
        public virtual Phong? Phong { get; set; }
        public virtual TienNghi? TienNghi { get; set; }
    }
}