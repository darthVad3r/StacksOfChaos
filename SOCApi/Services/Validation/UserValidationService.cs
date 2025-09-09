using System.Net.Mail;

namespace SOCApi.Services.Validation
{
    public static class UserValidationService
    {
        private const int MinimumPasswordLength = 6;

        public static void ValidateRegistrationInput(string userName, string password)
        {
            ValidateUsername(userName);
            ValidatePassword(password);
        }

        public static void ValidateUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(userName));
            }
            
            if (!IsValidEmail(userName))
            {
                throw new ArgumentException("Invalid email format", nameof(userName));
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }
            
            // Basic length validation - rely on Identity configuration for full validation
            if (password.Length < MinimumPasswordLength)
            {
                throw new ArgumentException($"Password must be at least {MinimumPasswordLength} characters long", nameof(password));
            }
        }

        public static void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
