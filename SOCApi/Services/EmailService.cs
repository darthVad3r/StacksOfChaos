using System.Data;
using Microsoft.Data.SqlClient;
using SOCApi.Interfaces;

namespace SOCApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task<bool> IsEmailUnique(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Email address is null or empty.");
                throw new ArgumentException("Email address cannot be null or empty.", nameof(email));
            }

            try
            {
                using var connection = new SqlConnection(Common.GetConnectionString());
                using var command = new SqlCommand(Common.StoredProcedures.VALIDATE_EMAIL_UNIQUE, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                var result = command.ExecuteScalar();
                int userCount = 0;
                if (result != null && result != DBNull.Value && int.TryParse(result.ToString(), out int count))
                {
                    userCount = count;
                }
                _logger.LogInformation("Checked email uniqueness for {Email}: {Count} users found.", email, userCount);
                // If userCount is 0, the email is unique
                // If userCount is greater than 0, the email is already in use
                _logger.LogInformation("Email uniqueness check for {Email} returned: {IsUnique}", email, userCount == 0);
                // Return true if no users found with the email, false otherwise
                return Task.FromResult(userCount == 0);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while checking email uniqueness: {Email}", email);
                throw new InvalidOperationException("An error occurred while checking email uniqueness.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking email uniqueness: {Email}", email);
                throw new InvalidOperationException("An error occurred while checking email uniqueness.", ex);
            }
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
