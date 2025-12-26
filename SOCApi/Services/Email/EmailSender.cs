using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SOCApi.Configuration;

namespace SOCApi.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                if (!IsValidEmail(email))
                {
                    throw new ArgumentException("Invalid email address.", nameof(email));
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Connect to SMTP server
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.UseSsl);

                // Authenticate if credentials are provided
                if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
                {
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                }

                // Send the email
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email} with subject: {Subject}", email, subject);
                throw;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}