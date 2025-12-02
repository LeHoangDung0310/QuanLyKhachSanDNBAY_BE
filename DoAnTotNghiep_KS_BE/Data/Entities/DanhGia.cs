using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDanhGia { get; set; }

        [Required]
        [ForeignKey("KhachHang")]
        public int MaKhachHang { get; set; }

        [ForeignKey("LoaiPhong")]
        public int? MaLoaiPhong { get; set; }

        [ForeignKey("DatPhong")]
        public int? MaDatPhong { get; set; }

        [Range(1, 5)]
        public int? Diem { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual NguoiDung? KhachHang { get; set; }
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual DatPhong? DatPhong { get; set; }
    }
}