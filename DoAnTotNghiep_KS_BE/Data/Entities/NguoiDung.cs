using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep_KS_BE.Data.Entities
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MatKhau { get; set; }

        [Required]
        [StringLength(20)]
        public string VaiTro { get; set; } = string.Empty;

        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(255)]
        public string? DiaChiChiTiet { get; set; }

        public int? MaPhuongXa { get; set; }

        [StringLength(255)]
        public string? AnhDaiDien { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "Hoạt động";

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? SoCCCD { get; set; }

        public DateTime? NgayCapCCCD { get; set; }

        [StringLength(200)]
        public string? NoiCapCCCD { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string? GioiTinh { get; set; }

        // Navigation properties
        [ForeignKey("MaPhuongXa")]
        public virtual PhuongXa? PhuongXa { get; set; }

        public virtual ICollection<OTP>? OTPs { get; set; }
        public virtual ICollection<DatPhong>? DatPhongs { get; set; }
        public virtual ICollection<HuyDatPhong>? HuyDatPhongs_NguoiDuyet { get; set; }
        public virtual ICollection<HoanTien>? HoanTiens_QuanTri { get; set; }
        public virtual ICollection<DanhGia>? DanhGias { get; set; }
        public virtual ICollection<TaiKhoanNganHang>? TaiKhoanNganHangs { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}