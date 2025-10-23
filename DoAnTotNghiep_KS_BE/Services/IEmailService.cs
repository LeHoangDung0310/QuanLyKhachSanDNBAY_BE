namespace DoAnTotNghiep_KS_BE.Services
{
    public interface IEmailService
    {
        Task<bool> SendOTPEmailAsync(string toEmail, string otpCode, string userName);
        Task<bool> SendResetPasswordOTPEmailAsync(string toEmail, string otpCode, string userName);
    }
}