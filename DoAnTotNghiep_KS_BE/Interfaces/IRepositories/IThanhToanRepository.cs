using DoAnTotNghiep_KS_BE.Interfaces.dto.ThanhToan;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IThanhToanRepository
    {
        // Tạo thanh toán mới
        Task<(bool success, string message, ThanhToanResponseDTO? data)> CreateThanhToanAsync(
            CreateThanhToanDTO createDTO,
            int nguoiThucHien);

        // Lấy thông tin thanh toán của đặt phòng
        Task<ThongTinThanhToanDTO?> GetThongTinThanhToanAsync(int maDatPhong);

        // Lấy lịch sử thanh toán
        Task<List<ThanhToanDTO>> GetLichSuThanhToanAsync(int maDatPhong);

        // Xác nhận thanh toán online (callback từ cổng thanh toán)
        Task<(bool success, string message)> XacNhanThanhToanOnlineAsync(int maThanhToan, string transactionId);

        // Hủy thanh toán
        Task<(bool success, string message)> HuyThanhToanAsync(int maThanhToan);
    }
}