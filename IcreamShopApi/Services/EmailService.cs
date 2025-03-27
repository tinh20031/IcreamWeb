using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace IcreamShopApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var client = new SmtpClient(smtpSettings["Server"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["SenderEmail"], smtpSettings["SenderPassword"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], "Ice Cream Shop"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false // Có thể đổi thành true nếu dùng HTML
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}