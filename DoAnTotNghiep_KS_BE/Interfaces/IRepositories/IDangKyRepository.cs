using DoAnTotNghiep_KS_BE.Interfaces.dto;

namespace DoAnTotNghiep_KS_BE.Interfaces.IRepositories
{
    public interface IDangKyRepository
    {
        Task<DangKyResponseDTO> DangKyAsync(DangKyDTO dangKyDTO);
        Task<XacThucOTPResponseDTO> XacThucOTPAsync(XacThucOTPDTO xacThucOTPDTO, string? ipAddress);
        Task<DangKyResponseDTO> GuiLaiOTPAsync(GuiLaiOTPDTO guiLaiOTPDTO);
    }
}