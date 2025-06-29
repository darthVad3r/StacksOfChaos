using SOCApi.Models;
using System.Text.Json;

namespace SOCApi.Interfaces
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;

        public UserService(ILogger<UserService> logger, IEmailService emailService, IPasswordService passwordService)
        {
            _logger = logger;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        public async Task<User> CreateNewUserAccountAsync(string userCredentials)
        {
            // Implementation for creating a new user account
            var newUser = JsonSerializer.Deserialize<User>(userCredentials);
            if (newUser == null)
            {
                _logger.LogError("Failed to deserialize user credentials.");
                throw new ArgumentException("Invalid user credentials provided.");
            }

            var username = newUser.Username;
            var password = newUser.Password;
            var emailAddress = newUser.EmailAddress;

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
            newUser.EmailAddress = emailAddress;
            newUser.CreatedAt = DateTime.UtcNow;
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

        public Task<User?> GetUserByEmailAsync(string email)
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
