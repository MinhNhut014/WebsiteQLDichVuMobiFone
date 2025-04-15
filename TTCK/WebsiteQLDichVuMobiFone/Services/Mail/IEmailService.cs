namespace WebsiteQLDichVuMobiFone.Services.Mail
{
    public interface IEmailService
    {
        public void SendOTP(string toEmail, string otpCode);
    }
}
