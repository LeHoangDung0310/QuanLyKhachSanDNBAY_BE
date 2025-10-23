using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DoAnTotNghiep_KS_BE.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendOTPEmailAsync(string toEmail, string otpCode, string userName)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"]
                ));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Mã xác thực OTP - Hotel Management System";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h2 style='color: #2c3e50;'>Xin chào {userName},</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>Hotel Management System</strong>.</p>
                            <p>Mã OTP của bạn là:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                                <h1 style='color: #007bff; letter-spacing: 5px; margin: 0;'>{otpCode}</h1>
                            </div>
                            <p style='color: #dc3545;'><strong>Lưu ý:</strong> Mã OTP này có hiệu lực trong <strong>5 phút</strong>.</p>
                            <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                            <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;'>
                            <p style='color: #6c757d; font-size: 12px;'>
                                Email này được gửi tự động, vui lòng không trả lời.<br>
                                © 2025 Hotel Management System. All rights reserved.
                            </p>
                        </div>
                    "
                };

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]!),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"OTP email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send OTP email to {toEmail}");
                return false;
            }
        }

        public async Task<bool> SendResetPasswordOTPEmailAsync(string toEmail, string otpCode, string userName)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"]
                ));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Mã xác thực đặt lại mật khẩu - Hotel Management System";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h2 style='color: #2c3e50;'>Xin chào {userName},</h2>
                            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn tại <strong>Hotel Management System</strong>.</p>
                            <p>Mã OTP để đặt lại mật khẩu của bạn là:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                                <h1 style='color: #dc3545; letter-spacing: 5px; margin: 0;'>{otpCode}</h1>
                            </div>
                            <p style='color: #dc3545;'><strong>Lưu ý:</strong> Mã OTP này có hiệu lực trong <strong>5 phút</strong>.</p>
                            <p style='color: #856404; background-color: #fff3cd; padding: 10px; border-radius: 5px;'>
                                ⚠️ Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này và đảm bảo tài khoản của bạn an toàn.
                            </p>
                            <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;'>
                            <p style='color: #6c757d; font-size: 12px;'>
                                Email này được gửi tự động, vui lòng không trả lời.<br>
                                © 2025 Hotel Management System. All rights reserved.
                            </p>
                        </div>
                    "
                };

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]!),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Reset password OTP email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reset password OTP email to {toEmail}");
                return false;
            }
        }
    }
}