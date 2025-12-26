using System.Runtime.CompilerServices;

namespace SOCApi.Services.Email
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string userName);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendNotificationEmailAsync(string toEmail, string subject, string message);
        Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
    }
}