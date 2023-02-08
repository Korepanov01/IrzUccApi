using IrzUccApi.Models.Configurations;
using MailKit.Net.Smtp;
using MimeKit;

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
            var subject = "Вы зарегистрированы в ЕЦК ИРЗ!";
            var message = $"Ваш пароль: {password}.";
            await SendEmailAsync(email, subject, message);
        }

        public async Task SendResetPasswordMessage(string email, string url)
        {
            var subject = "Восстановление пароля";
            var message = $"Перейдите по ссылке, чтобы восстановить пароль.\n{url}";
            await SendEmailAsync(email, subject, message);
        }

        public async Task SendNewPasswordMessage(string email, string password)
        {
            var subject = "Восстановление пароля";
            var message = $"Ваш новый пароль: {password}";
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
