using System.Runtime.CompilerServices;

namespace SOCApi.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string to, string subject, string body);
        public void SendPasswordResetEmail(string emailAddress);
        public void SendWelcomeEmail(string emailAddress, string userName);
        public void SendAccountDeletionConfirmationEmail(string emailAddress);
        public bool ValidateEmailFormat(string emailAddress);
        public Task<bool> IsEmailUnique(string email);
        // The IsEmailUnique method is asynchronous to allow for database or external service checks.
        // It returns a Task<bool> to indicate whether the email is unique or not.
        // This is useful for scenarios like user registration where you need to ensure the email is not already in use.
        public void SendEmailVerification(string emailAddress, string verificationCode);
        public void SendEmailChangeNotification(string oldEmail, string newEmail, string userName);
        public Task<bool> UpdateUserEmailAsync(int userId, string newEmail);

    }
}
