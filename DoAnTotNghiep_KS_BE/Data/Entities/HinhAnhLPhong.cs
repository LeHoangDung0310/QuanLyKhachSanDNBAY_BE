using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("HinhAnhLPhong")]
    public class HinhAnhLPhong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHinhAnh { get; set; }

        [Required]
        [ForeignKey("LoaiPhong")]
        public int MaLoaiPhong { get; set; }

        [StringLength(255)]
        public string? Url { get; set; }

        // Navigation properties
        public virtual LoaiPhong? LoaiPhong { get; set; }
    }
}