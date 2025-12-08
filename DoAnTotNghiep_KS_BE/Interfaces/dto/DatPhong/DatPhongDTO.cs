using System;
using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong
{
    public class DatPhongDTO
    {
        public int MaDatPhong { get; set; }
        public int MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; }
        public string? EmailKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhanPhong { get; set; }
        public DateTime NgayTraPhong { get; set; }
        public int SoNgayO { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public List<PhongDatDTO>? DanhSachPhong { get; set; }
        public decimal TongTien { get; set; }

        // THÊM MỚI
        public int? MaNguoiTao { get; set; }
        public string? TenNguoiTao { get; set; } // Tên lễ tân tạo
        public string LoaiDatPhong { get; set; } = "Online"; // "Online" hoặc "TrucTiep"                                  
        public DateTime? ThoiGianCheckIn { get; set; }
        public DateTime? ThoiGianCheckOut { get; set; }
    }

    public class PhongDatDTO
    {
        public int MaPhong { get; set; }
        public string? SoPhong { get; set; }
        public string? TenLoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public int SoNguoi { get; set; }
    }
}