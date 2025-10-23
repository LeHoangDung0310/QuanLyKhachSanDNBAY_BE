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

        [StringLength(100)]
        public string? TenLoai { get; set; }

        public int? DienTich { get; set; }

        public int? SoGiuong { get; set; }

        [StringLength(100)]
        public string? HuongNhin { get; set; }

        public string? MoTa { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? GiaMoiDem { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Trong";

        [ForeignKey("Tang")]
        public int? MaTang { get; set; }

        // Navigation properties
        public virtual Tang? Tang { get; set; }
        public virtual ICollection<Phong_TienNghi>? Phong_TienNghis { get; set; }
        public virtual ICollection<HinhAnhPhong>? HinhAnhPhongs { get; set; }
        public virtual ICollection<DatPhong>? DatPhongs { get; set; }
        public virtual ICollection<DanhGia>? DanhGias { get; set; }
    }
}