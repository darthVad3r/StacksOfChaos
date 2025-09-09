using SOCApi.Services.Validation;

namespace SOCApi.Services.Password
{
    /// <summary>
    /// Facade service that provides a unified interface for password operations
    /// while delegating to specialized services following SOLID principles
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IPasswordValidationService _passwordValidationService;
        private readonly IPasswordManagementService _passwordManagementService;

        public PasswordService(
            IPasswordHashingService passwordHashingService,
            IPasswordValidationService passwordValidationService,
            IPasswordManagementService passwordManagementService)
        {
            _passwordHashingService = passwordHashingService;
            _passwordValidationService = passwordValidationService;
            _passwordManagementService = passwordManagementService;
        }

        // Delegate to hashing service
        public string HashPassword(string password)
        {
            return _passwordHashingService.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return _passwordHashingService.VerifyPassword(password, hashedPassword);
        }

        // Delegate to validation service
        public async Task<bool> ValidatePasswordAsync(string password)
        {
            return await _passwordValidationService.ValidatePasswordAsync(password);
        }

        public async Task<bool> IsPasswordStrongEnoughAsync(string password)
        {
            return await _passwordValidationService.IsPasswordStrongEnoughAsync(password);
        }

        // Delegate to management service
        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            return await _passwordManagementService.ChangePasswordAsync(userId, currentPassword, newPassword);
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            return await _passwordManagementService.ResetPasswordAsync(userId, newPassword);
        }

        public async Task<bool> IsPasswordPreviouslyUsedAsync(string userId, string password)
        {
            return await _passwordManagementService.IsPasswordPreviouslyUsedAsync(userId, password);
        }
    }
}