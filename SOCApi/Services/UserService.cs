using SOCApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using SOCApi.Interfaces;

namespace SOCApi.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly string _connectionString;

        public UserService(ILogger<UserService> logger,
            IEmailService emailService,
            IPasswordService passwordService,
            string connectionString)
        {
            _logger = logger;
            _emailService = emailService;
            _passwordService = passwordService;
            string getConnectionString = Common.GetConnectionString();
            _connectionString = getConnectionString;
            _connectionString = connectionString;
        }

        public async Task<User> CreateNewUserAccountAsync(string userCredentials)
        {
            // Implementation for creating a new user account
            var newUser = JsonSerializer.Deserialize<UserCredentials>(userCredentials);
            if (newUser == null)
            {
                _logger.LogError("Failed to deserialize user credentials.");
                throw new ArgumentException("Invalid user credentials provided.");
            }

            var username = newUser.Username;
            var password = newUser.Password;
            var emailAddress = newUser.Emaild;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(emailAddress))
            {
                _logger.LogError("User tried to create an account with a missing username, email address or password");
                throw new ArgumentException("Username, password, and email address cannot be empty.");
            }

            if (!await IsUsernameUnique(username))
            {
                // This is a blocking call, consider using async all the way up
                _logger.LogWarning($"Username '{username}' is already taken.");
                throw new InvalidOperationException($"Username '{username}' is already taken.");
            }

            if (!await _emailService.IsEmailUnique(emailAddress))
            {
                _logger.LogWarning($"Email '{emailAddress}' is already registered.");
                throw new InvalidOperationException($"Email '{emailAddress}' is already registered.");
            }

            newUser.Username = username;
            newUser.Email = emailAddress;
            newUser.Password = _passwordService.HashPassword(password);

            return await Task.FromResult(newUser);
        }

        public Task<bool> DeleteUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByEmailAsync(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(Common.StoredProcedures.GetOrCreateUser, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("Email")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        // Map other properties as needed
                    };
                }
            }
            return null;
        }

        public Task<User?> GetUserByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUsernameUnique(string username)
        {
            throw new NotImplementedException();
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
