using Microsoft.AspNetCore.Identity;
using SOCApi.Services.Validation;
using SOCApi.Services.Password;
using SOCApi.Models;

namespace SOCApi.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<Models.User> _userManager;
        private readonly IUserRetrievalService _userRetrievalService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<Models.User> userManager, 
            IUserRetrievalService userRetrievalService,
            IPasswordManagementService passwordManagementService,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _userRetrievalService = userRetrievalService;
            _passwordManagementService = passwordManagementService;
            _logger = logger;
        }

        public async Task<Models.User> RegisterRequest(string userName, string password, string? firstName = null, string? lastName = null)
        {
            try
            {
                // Centralized validation
                UserValidationService.ValidateRegistrationInput(userName, password);
                
                await EnsureUsernameIsAvailable(userName);
                
                // Create the user
                return await CreateNewUserAsync(userName, password, firstName, lastName);
            }
            catch (ArgumentException ex)
            {
                // Handle validation exceptions specifically
                _logger.LogError(ex, "User registration failed due to invalid input");
                throw new ArgumentException("Invalid registration input", ex);
            }
            catch (InvalidOperationException ex)
            {
                // Handle business logic exceptions specifically
                _logger.LogError(ex, "User registration failed for {UserName}", userName);
                throw new InvalidOperationException("User registration failed", ex);
            }          
        }

        private async Task<Models.User> CreateNewUserAsync(string userName, string password, string? firstName = null, string? lastName = null)
        {
            var user = new Models.User 
            { 
                UserName = userName,
                Email = userName, // Best practice: use email as username
                EmailConfirmed = false, // Should be confirmed via email
                FirstName = firstName,
                LastName = lastName
            };
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                return user;
            }
            
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        private async Task EnsureUsernameIsAvailable(string userName)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(userName);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Username already exists");
                }
            }
            catch (InvalidOperationException)
            {
                // Re-throw business logic exceptions without modification
                throw new InvalidOperationException("Error checking username availability", new ArgumentException("Username already exists"));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error checking username availability", ex);
            }
        }

        public async Task<Models.User> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var success = await _passwordManagementService.ChangePasswordAsync(userId, currentPassword, newPassword);
            if (!success)
            {
                throw new InvalidOperationException("Password change failed");
            }

            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found after password change");
            }
            return user;
        }

        public async Task<Models.User?> GetUserByNameAsync(string userName)
        {
            return await _userRetrievalService.GetUserByNameAsync(userName);
        }

        public async Task<Models.User?> GetUserByIdAsync(string userId)
        {
            return await _userRetrievalService.GetUserByIdAsync(userId);
        }

        public async Task<IEnumerable<Models.User>> GetAllUsersAsync()
        {
            return await _userRetrievalService.GetAllUsersAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(Models.User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<Models.User> UpdateUserAsync(Models.User user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }
            
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User update failed: {errors}");
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            
            return await _userManager.DeleteAsync(user);
        }
    }
}