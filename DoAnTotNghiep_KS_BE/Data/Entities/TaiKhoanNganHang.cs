using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("TaiKhoanNganHang")]
    public class TaiKhoanNganHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTaiKhoan { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int MaNguoiDung { get; set; }

        [StringLength(100)]
        public string? NganHang { get; set; }

        [StringLength(30)]
        public string? SoTaiKhoan { get; set; }

        [StringLength(100)]
        public string? TenChuTK { get; set; }

        // Navigation properties
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}