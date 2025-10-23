using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaRefreshToken { get; set; } // ← Đổi từ Id sang MaRefreshToken

        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime NgayHetHan { get; set; }

        [StringLength(100)]
        public string? DiaChi { get; set; }

        public bool DaSuDung { get; set; }
        public DateTime? NgaySuDung { get; set; }

        // Navigation property
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}