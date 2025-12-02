using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("LoaiPhong")]
    public class LoaiPhong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLoaiPhong { get; set; }

        [Required]
        [StringLength(100)]
        public string TenLoaiPhong { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public int? SoNguoiToiDa { get; set; }

        public int? SoGiuong { get; set; }

        public int? DienTich { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? GiaMoiDem { get; set; }

        // Navigation properties
        public virtual ICollection<Phong>? Phongs { get; set; }
        public virtual ICollection<HinhAnhLPhong>? HinhAnhLPhongs { get; set; }
        public virtual ICollection<DanhGia>? DanhGias { get; set; }
    }
}