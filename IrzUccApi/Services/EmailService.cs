using MimeKit;
using MailKit.Net.Smtp;
using System.Configuration;
using IrzUccApi.Models.Configurations;

namespace IrzUccApi.Services
{
    public class EmailService
    {
        private readonly EmailConfiguration _configuration;

        public EmailService(EmailConfiguration configuration)
        {
            _configuration = configuration;        
        }

        public async Task SendRegisterMessage(string email, string password)
        {
            var subject = "Вы зарегестрированы в ЕЦК ИРЗ!";
            var message = $"Ваш пароль: {password}.";
            await SendEmailAsync(email, subject, message);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ЕЦК ИРЗ", ""));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };


            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration.Host, _configuration.Port, _configuration.UseSsl);
            await client.AuthenticateAsync(_configuration.UserName, _configuration.Password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
