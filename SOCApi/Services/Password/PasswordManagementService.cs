using Microsoft.AspNetCore.Identity;
using SOCApi.Data;
using SOCApi.Models;
using Microsoft.EntityFrameworkCore;

namespace SOCApi.Services.Password
{
    /// <summary>
    /// Service for managing password operations like changing and resetting passwords
    /// </summary>
    public class PasswordManagementService : IPasswordManagementService
    {
        private readonly UserManager<SOCApi.Models.User> _userManager;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly SocApiDbContext _context;
        private readonly ILogger<PasswordManagementService> _logger;

        public PasswordManagementService(
            UserManager<Models.User> userManager,
            IPasswordHashingService passwordHashingService,
            SocApiDbContext context,
            ILogger<PasswordManagementService> logger)
        {
            _userManager = userManager;
            _passwordHashingService = passwordHashingService;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for password change", userId);
                    return false;
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                
                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                    return true;
                }

                _logger.LogWarning("Password change failed for user {UserId}: {Errors}", 
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for password reset", userId);
                    return false;
                }

                // Generate reset token and reset password
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Password reset successfully for user {UserId}", userId);
                    return true;
                }

                _logger.LogWarning("Password reset failed for user {UserId}: {Errors}", 
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> IsPasswordPreviouslyUsedAsync(string userId, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Check against current password
                if (_passwordHashingService.VerifyPassword(password, user.PasswordHash ?? string.Empty))
                {
                    return true;
                }

                // TODO: Implement password history tracking if required
                // This would require storing previous password hashes in a separate table
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking password history for user {UserId}", userId);
                return false;
            }
        }
    }
}