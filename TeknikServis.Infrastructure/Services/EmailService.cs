using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using TeknikServis.Application.Interfaces;
using TeknikServis.Application.Common.Models; // MailSettings burada olmalı

namespace TeknikServis.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _settings;
        public EmailService(IOptions<MailSettings> settings) => _settings = settings.Value;

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.UserName));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            // Hata veren kısım: MailKit olduğunu açıkça belirtiyoruz
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}