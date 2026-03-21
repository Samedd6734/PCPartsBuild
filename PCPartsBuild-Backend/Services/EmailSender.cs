using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit; // MailKit'in mesaj yapısı
using MailKit.Net.Smtp; // MailKit'in SMTP istemcisi
using System.Threading.Tasks;

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
            // appsettings.json'dan verileri çek
            var host = _configuration["MailSettings:Host"];
            var port = int.Parse(_configuration["MailSettings:Port"]);
            var userName = _configuration["MailSettings:Username"];
            var password = _configuration["MailSettings:Password"];
            var senderEmail = _configuration["MailSettings:SenderEmail"];
            var senderName = _configuration["MailSettings:SenderName"];

            // MimeMessage oluştur (System.Net.Mail yerine bu kullanılır)
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // SMTP Bağlantısı (MailKit ile)
            using (var client = new SmtpClient())
            {
                // Oracle sertifika sorunları çıkarırsa bu satır güvenliği esnetir (Geliştirme aşaması için)
                client.CheckCertificateRevocation = false;

                try
                {
                    // 1. Bağlan (Port 587 genelde StartTls kullanır)
                    await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);

                    // 2. Kimlik Doğrula
                    await client.AuthenticateAsync(userName, password);

                    // 3. Gönder
                    await client.SendAsync(emailMessage);

                    // 4. Bağlantıyı Kes
                    await client.DisconnectAsync(true);
                }
                catch (System.Exception ex)
                {
                    // Hata olursa konsola yazsın (debug için)
                    System.Console.WriteLine($"MAIL HATASI: {ex.Message}");
                    throw; // Hatayı fırlat ki kod akışı dursun
                }
            }
        }
    }
}