using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaThanhToan { get; set; }

        [Required]
        [ForeignKey("DatPhong")]
        public int MaDatPhong { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? SoTien { get; set; }

        [StringLength(50)]
        public string? PhuongThuc { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "DangCho";

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual DatPhong? DatPhong { get; set; }
    }
}