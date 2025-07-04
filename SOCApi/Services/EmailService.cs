using SOCApi.Interfaces;

namespace SOCApi.Services
{
    public class EmailService : IEmailService
    {
        public Task<bool> IsEmailUnique(string email)
        {
            throw new NotImplementedException();
        }

        public void SendAccountDeletionConfirmationEmail(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public void SendEmailChangeNotification(string oldEmail, string newEmail, string userName)
        {
            throw new NotImplementedException();
        }

        public void SendEmailVerification(string emailAddress, string verificationCode)
        {
            throw new NotImplementedException();
        }

        public void SendPasswordResetEmail(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public void SendWelcomeEmail(string emailAddress, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserEmailAsync(int userId, string newEmail)
        {
            throw new NotImplementedException();
        }

        public bool ValidateEmailFormat(string emailAddress)
        {
            throw new NotImplementedException();
        }
    }
}
