using DoAnTotNghiep_KS_BE.Interfaces.dto.HuyDatPhong;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IHuyDatPhongRepository
    {
        // Người dùng yêu cầu hủy
        Task<(bool success, string message, decimal? phiGiu)> YeuCauHuyDatPhongAsync(int maDatPhong, string lyDo, int maNguoiDung);

        // Lấy danh sách yêu cầu hủy (Lễ tân)
        Task<List<HuyDatPhongDTO>> GetAllAsync();

        // Lấy chi tiết yêu cầu hủy
        Task<HuyDatPhongDTO?> GetByIdAsync(int maHuyDatPhong);

        // Lấy danh sách yêu cầu hủy của khách hàng
        Task<List<HuyDatPhongDTO>> GetByKhachHangAsync(int maKhachHang);

        // Lễ tân duyệt/từ chối yêu cầu hủy
        Task<(bool success, string message)> DuyetHuyDatPhongAsync(int maHuyDatPhong, bool choDuyet, int maLeTan, string? ghiChu);

        // Kiểm tra điều kiện hủy
        Task<(bool canCancel, string message, decimal phiGiu, decimal tienHoan)> KiemTraDieuKienHuyAsync(int maDatPhong);
    }
}