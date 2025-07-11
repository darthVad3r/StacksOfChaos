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
        public void SendEmailVerification(string emailAddress, string verificationCode);
        public void SendEmailChangeNotification(string oldEmail, string newEmail, string userName);
        public Task<bool> UpdateUserEmailAsync(int userId, string newEmail);

    }
}
