using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("Phong")]
    public class Phong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhong { get; set; }

        [StringLength(10)]
        public string? SoPhong { get; set; }

        public int? SoGiuong { get; set; }

        public int? SoNguoiToiDa { get; set; }

        public string? MoTa { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Trong";

        [ForeignKey("Tang")]
        public int? MaTang { get; set; }

        [ForeignKey("LoaiPhong")]
        public int? MaLoaiPhong { get; set; }

        // Navigation properties
        public virtual Tang? Tang { get; set; }
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ICollection<Phong_TienNghi>? Phong_TienNghis { get; set; }
        public virtual ICollection<DatPhong_Phong>? DatPhong_Phongs { get; set; }
    }
}