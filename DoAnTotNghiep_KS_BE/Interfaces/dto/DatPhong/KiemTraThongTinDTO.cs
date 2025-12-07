namespace DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong
{
    public class KiemTraThongTinDTO
    {
        public bool DayDuThongTin { get; set; }
        public List<string> ThongTinThieu { get; set; } = new();
        public string? Message { get; set; }
    }
}