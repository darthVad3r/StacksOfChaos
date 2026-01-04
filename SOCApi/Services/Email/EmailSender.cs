namespace SOCApi.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string RecipeintName, string htmlMessage)
        {
            // TODO: Implement actual email sending logic
            // For now, just log the email that would be sent
            _logger.LogInformation("Email would be sent to {Email} with subject: {Subject}", email, subject);
            _logger.LogDebug("Email content: {Content}", htmlMessage);
            
            return Task.CompletedTask;
        }
    }
}