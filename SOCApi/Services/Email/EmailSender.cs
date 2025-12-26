namespace SOCApi.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                IsValidEmail(email);

                // TODO: Implement actual email sending logic
                // For now, just log the email that would be sent
                _logger.LogInformation("Email would be sent to {Email} with subject: {Subject}", email, subject);
                _logger.LogDebug("Email content: {Content}", htmlMessage);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendEmailAsync parameters validation.");
                throw;
            }
        }

        private static void IsValidEmail(string email)
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