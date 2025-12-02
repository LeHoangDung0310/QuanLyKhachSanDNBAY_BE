using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("DatPhong")]
    public class DatPhong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaDatPhong { get; set; }

        [Required]
        [ForeignKey("KhachHang")]
        public int MaKhachHang { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "date")]
        public DateTime NgayNhanPhong { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime NgayTraPhong { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "ChoDuyet";

        // Navigation properties
        public virtual NguoiDung? KhachHang { get; set; }
        public virtual ICollection<DatPhong_Phong>? DatPhong_Phongs { get; set; }
        public virtual ICollection<ThanhToan>? ThanhToans { get; set; }
        public virtual ICollection<HuyDatPhong>? HuyDatPhongs { get; set; }
        public virtual ICollection<DanhGia>? DanhGias { get; set; }
    }
}