using System.Net.Mail;
using System.Net;

namespace HMZ_rt.Controllers
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
    }
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
