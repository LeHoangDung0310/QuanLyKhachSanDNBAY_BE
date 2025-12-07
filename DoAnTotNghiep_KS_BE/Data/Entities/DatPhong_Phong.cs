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

        [Required]
        [Range(1, 10)]
        public int SoNguoi { get; set; } = 1;

        // Navigation properties
        public virtual DatPhong? DatPhong { get; set; }
        public virtual Phong? Phong { get; set; }
    }
}