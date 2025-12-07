using System;

namespace DoAnTotNghiep_KS_BE.Interfaces.dto.ThanhToan
{
    public class ThanhToanDTO
    {
        public int MaThanhToan { get; set; }
        public int MaDatPhong { get; set; }
        public decimal? SoTien { get; set; }
        public string? PhuongThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class CreateThanhToanDTO
    {
        public int MaDatPhong { get; set; }
        public decimal SoTien { get; set; }
        public string PhuongThuc { get; set; } = string.Empty; // TienMat, ChuyenKhoan, TheATM, MoMo, ZaloPay
        public string? GhiChu { get; set; }
    }

    public class ThanhToanResponseDTO
    {
        public int MaThanhToan { get; set; }
        public int MaDatPhong { get; set; }
        public decimal SoTienThanhToan { get; set; }
        public decimal TongTienDatPhong { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConLai { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? Message { get; set; }
    }

    public class ThongTinThanhToanDTO
    {
        public int MaDatPhong { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConLai { get; set; }
        public List<ThanhToanDTO> DanhSachThanhToan { get; set; } = new();
    }
}