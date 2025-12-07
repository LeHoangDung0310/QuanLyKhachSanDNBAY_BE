using DoAnTotNghiep_KS_BE.Interfaces.dto.DatPhong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IDatPhongRepository
    {
        // Kiểm tra thông tin người dùng
        Task<KiemTraThongTinDTO> KiemTraThongTinNguoiDungAsync(int maNguoiDung);

        // Kiểm tra phòng trống
        Task<List<int>> KiemTraPhongTrongAsync(List<int> danhSachMaPhong, DateTime ngayNhan, DateTime ngayTra);

        // Tạo đặt phòng (khách hàng online)
        Task<(bool success, string message, int? maDatPhong)> CreateDatPhongAsync(int maNguoiDung, CreateDatPhongDTO createDTO);

        // Tạo đặt phòng trực tiếp (lễ tân)
        Task<(bool success, string message, DatPhongTrucTiepResponseDTO? data)> CreateDatPhongTrucTiepAsync(
            int maLeTan,
            CreateDatPhongTrucTiepDTO createDTO);

        // Lấy danh sách đặt phòng của khách hàng
        Task<List<DatPhongDTO>> GetDatPhongByKhachHangAsync(int maKhachHang);

        // Lấy chi tiết đặt phòng
        Task<DatPhongDTO?> GetDatPhongByIdAsync(int maDatPhong);

        // Hủy đặt phòng
        Task<(bool success, string message)> HuyDatPhongAsync(int maDatPhong, int maNguoiDung);

        // Lấy tất cả đặt phòng (Admin/LeTan)
        Task<List<DatPhongDTO>> GetAllDatPhongAsync();

        // Check-in
        Task<(bool success, string message)> CheckInAsync(int maDatPhong, int maLeTan);

        // Check-out
        Task<(bool success, string message)> CheckOutAsync(int maDatPhong, int maLeTan);
    }
}