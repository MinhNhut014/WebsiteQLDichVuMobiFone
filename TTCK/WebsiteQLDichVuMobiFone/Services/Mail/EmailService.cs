using MailKit.Net.Smtp;
using MimeKit;

namespace WebsiteQLDichVuMobiFone.Services.Mail
{
    public class EmailService : IEmailService
    {
        public void SendOTP(string toEmail, string otpCode)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("dangminhnhut1123@gmail.com")); // Thay bằng Gmail của bạn
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Mã OTP xác thực";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"Mã OTP của bạn là: {otpCode}"
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("dangminhnhut1123@gmail.com", "wbzn aqdj xaja ttlp"); // Thay bằng Gmail + Mật khẩu ứng dụng
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
