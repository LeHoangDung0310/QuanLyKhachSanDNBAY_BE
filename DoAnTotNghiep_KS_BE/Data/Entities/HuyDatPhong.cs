using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("HuyDatPhong")]
    public class HuyDatPhong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHuyDatPhong { get; set; }

        [Required]
        public int MaDatPhong { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        public int? MaNguoiDuyet { get; set; }

        public DateTime NgayYeuCau { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? LyDo { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "ChoDuyet";

        [Column(TypeName = "decimal(12,2)")]
        public decimal? PhiGiu { get; set; }

        public DateTime? NgayXuLy { get; set; }

        [StringLength(50)]
        public string? SoTaiKhoan { get; set; }

        [StringLength(100)]
        public string? TenNganHang { get; set; }

        [StringLength(100)]
        public string? TenChuTaiKhoan { get; set; }

        [StringLength(255)]
        public string? GhiChuNguoiDuyet { get; set; }

        // Navigation properties - GIỮ REFERENCE ĐƠN GIẢN
        [ForeignKey("MaDatPhong")]
        public virtual DatPhong? DatPhong { get; set; }

        [ForeignKey("MaKhachHang")]
        public virtual NguoiDung? KhachHang { get; set; }

        [ForeignKey("MaNguoiDuyet")]
        public virtual NguoiDung? NguoiDuyet { get; set; }

        public virtual ICollection<HoanTien>? HoanTiens { get; set; }
    }
}