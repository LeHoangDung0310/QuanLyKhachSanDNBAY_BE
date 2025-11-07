using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("DatPhong_Phong")]
    public class DatPhong_Phong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDatPhong_Phong { get; set; }

        [Required]
        [ForeignKey("DatPhong")]
        public int MaDatPhong { get; set; }

        [Required]
        [ForeignKey("Phong")]
        public int MaPhong { get; set; }

        // Navigation properties
        public virtual DatPhong? DatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
    }
}