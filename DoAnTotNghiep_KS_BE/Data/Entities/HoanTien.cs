using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("HoanTien")]
    public class HoanTien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHoanTien { get; set; }

        [Required]
        [ForeignKey("HuyDatPhong")]
        public int MaHuyDatPhong { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "ChoXuLy";

        [ForeignKey("QuanTri")]
        public int? MaQuanTri { get; set; }

        public DateTime? NgayXuLy { get; set; }

        // Navigation properties
        public virtual HuyDatPhong? HuyDatPhong { get; set; }
        public virtual NguoiDung? QuanTri { get; set; }
    }
}