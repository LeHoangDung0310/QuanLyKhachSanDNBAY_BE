using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("HinhAnhPhong")]
    public class HinhAnhPhong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHinhAnh { get; set; }

        [Required]
        [ForeignKey("Phong")]
        public int MaPhong { get; set; }

        [StringLength(255)]
        public string? Url { get; set; }

        // Navigation properties
        public virtual Phong? Phong { get; set; }
    }
}