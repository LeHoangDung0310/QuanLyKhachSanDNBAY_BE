using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("OTP")]
    public class OTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaOTP { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int MaNguoiDung { get; set; }

        [StringLength(10)]
        public string? MaXacThuc { get; set; }

        [StringLength(50)]
        public string? Loai { get; set; }

        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        public DateTime? HetHanSau { get; set; }

        public bool DaSuDung { get; set; } = false;

        // Navigation properties
        public virtual NguoiDung? NguoiDung { get; set; }
    }
}