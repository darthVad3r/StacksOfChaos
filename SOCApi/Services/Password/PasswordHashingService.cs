using BCrypt.Net;

namespace SOCApi.Services.Password
{
    /// <summary>
    /// Service for password hashing and verification using BCrypt
    /// </summary>
    public class PasswordHashingService : IPasswordHashingService
    {
        private const int WorkFactor = 12; // BCrypt work factor for security

        public Task<string> GeneratePasswordResetTokenAsync()
        {
            throw new NotImplementedException();
        }

        public string GenerateRandomPassword(int length = 12)
        {
            throw new NotImplementedException();
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public Task InvalidatePasswordResetTokenAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidatePasswordResetTokenAsync(string token, string userId)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }
            
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) for debugging purposes
                Console.WriteLine($"Password verification failed: {ex.Message}");
                // If verification fails due to invalid hash format, return false
                return false;
            }
        }
    }
}