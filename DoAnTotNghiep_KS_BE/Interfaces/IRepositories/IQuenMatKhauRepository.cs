using DoAnTotNghiep_KS_BE.Interfaces.dto;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IQuenMatKhauRepository
    {
        Task<QuenMatKhauResponseDTO> GuiOTPQuenMatKhauAsync(QuenMatKhauDTO quenMatKhauDTO);
        Task<XacThucOTPQuenMatKhauResponseDTO> XacThucOTPQuenMatKhauAsync(XacThucOTPQuenMatKhauDTO xacThucOTPDTO);
        Task<DatLaiMatKhauResponseDTO> DatLaiMatKhauAsync(DatLaiMatKhauDTO datLaiMatKhauDTO);
    }
}