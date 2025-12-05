using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace PCPartsAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // 1. Ayarları Çek
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var username = _configuration["EmailSettings:Username"]; // Yeni eklediğimiz alan (Login için)
            var fromEmail = _configuration["EmailSettings:FromEmail"]; // Görünen mail adresi
            var password = _configuration["EmailSettings:Password"];

            // 2. İstemciyi Hazırla (Login olurken Username kullanıyoruz)
            var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            // 3. Mesajı Oluştur (Gönderen kısmında FromEmail kullanıyoruz)
            var mailMessage = new MailMessage(fromEmail, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}