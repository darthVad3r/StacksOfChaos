using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SOCApi.Configuration;

namespace SOCApi.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _environment;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings, IWebHostEnvironment environment)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
            _environment = environment;
        }

        public async Task SendEmailAsync(string email, string subject, string recipientName, string htmlMessage)
        {
            try
            {
                if (!IsValidEmail(email))
                {
                    throw new ArgumentException("Invalid email address.", nameof(email));
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(recipientName, email));
                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Only accept all SSL certificates in development environment
                if (_environment.IsDevelopment())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, 
                    _emailSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                // Authenticate if credentials are provided
                if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
                {
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject: {Subject}", email, subject);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address cannot be empty.", nameof(email));
            }

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