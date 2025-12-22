using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SOCApi.Services.Validation;
using SOCApi.Models;

namespace SOCApi.Services
{
    public interface IUserRetrievalService
    {
        Task<Models.User?> GetUserByNameAsync(string userName);
        Task<Models.User?> GetUserByIdAsync(string userId);
        Task<IEnumerable<Models.User>> GetAllUsersAsync();
    }

    public class UserRetrievalService : IUserRetrievalService
    {
        private readonly UserManager<Models.User> _userManager;

        public UserRetrievalService(UserManager<Models.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Models.User?> GetUserByNameAsync(string userName)
        {
            UserValidationService.ValidateUsername(userName);
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<Models.User?> GetUserByIdAsync(string userId)
        {
            UserValidationService.ValidateUserId(userId);
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<Models.User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
