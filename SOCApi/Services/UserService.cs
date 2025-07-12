using SOCApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using SOCApi.Interfaces;
using SOCApi.Exceptions;

namespace SOCApi.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public UserService(ILogger<UserService> logger,
            IEmailService emailService,
            IPasswordService passwordService,
            string connectionString)
        {
            _logger = logger;
            _emailService = emailService;
            _passwordService = passwordService;
            _connectionString = connectionString;
        }

        public async Task<UserCreatedResponse> CreateNewUserAccountAsync(UserCredentials newUserCredentials)
        {
            if (newUserCredentials == null)
            {
                _logger.LogError("Failed to deserialize user credentials.");
                throw new ArgumentException("Invalid user credentials provided.");
            }

            var username = newUserCredentials.Username;
            var password = newUserCredentials.Password;
            var emailAddress = newUserCredentials.Email;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(emailAddress))
            {
                _logger.LogError("User tried to create an account with a missing username, email address or password");
                throw new ArgumentException("Username, password, and email address cannot be empty.");
            }

            if (!await IsUsernameUnique(username))
            {
                _logger.LogWarning($"Username '{username}' is already taken.");
                throw new DuplicateUsernameException(username);
            }

            if (!await _emailService.IsEmailUnique(emailAddress))
            {
                _logger.LogWarning($"Email '{emailAddress}' is already registered.");
                throw new DuplicateEmailException(emailAddress);
            }

            var passwordHash = _passwordService.HashPassword(password);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(Common.StoredProcedures.CREATE_USER, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", passwordHash);
            command.Parameters.AddWithValue("@Email", emailAddress);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                _logger.LogError("Failed to create user account.");
                throw new InvalidOperationException("Failed to create user account.");
            }

            return new UserCreatedResponse
            {
                Id = Convert.ToInt32(result),
                Email = emailAddress,
                Username = username
            };
        }

        public Task<User> CreateNewUserAccountAsync(string userCredentials)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(Common.StoredProcedures.GET_USERS_BY_EMAIL, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            // Map other properties as needed
                        };
                    }
                }
                return null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving user by email: {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user by email: {Email}", email);
                throw;
            }
        }

        public Task<User?> GetUserByEmailAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUsernameUnique(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogError("Username is null or empty.");
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(Common.StoredProcedures.VALIDATE_USERNAME_UNIQUE, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", username);
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                int userCount = 0;

                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    userCount = count;
                }
                return userCount == 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while checking username uniqueness: {Username}", username);
                throw new InvalidOperationException("An error occurred while checking username uniqueness.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking username uniqueness: {Username}", username);
                throw new InvalidOperationException("An error occurred while checking username uniqueness.", ex);
            }
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
