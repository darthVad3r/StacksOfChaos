namespace SOCApi.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;

        public EmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to Our Service!";
            var message = $"Hello {userName},<br/>Welcome to our service. We're glad to have you!";
            await _emailSender.SendEmailAsync(toEmail, subject, message);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Password Reset Request";
            var message = $"Click <a href='{resetLink}'>here</a> to reset your password.";
            await _emailSender.SendEmailAsync(toEmail, subject, message);
        }

        public async Task SendNotificationEmailAsync(string toEmail, string subject, string message)
        {
            await _emailSender.SendEmailAsync(toEmail, subject, message);
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            var subject = "Please Confirm Your Email";
            var message = $"Click <a href='{confirmationLink}'>here</a> to confirm your email address.";
            await _emailSender.SendEmailAsync(toEmail, subject, message);
        }
    }
}