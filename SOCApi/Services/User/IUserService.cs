using Microsoft.AspNetCore.Identity;
using SOCApi.Models;

namespace SOCApi.Services.User
{
    public interface IUserService
    {
        Task<Models.User> RegisterRequest(string userName, string password, string? firstName = null, string? lastName = null);
        Task<Models.User> UpdateUserAsync(Models.User user);
        Task<Models.User> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<Models.User?> GetUserByNameAsync(string userName);
        Task<Models.User?> GetUserByIdAsync(string userId);
        Task<IEnumerable<Models.User>> GetAllUsersAsync();
        Task<IdentityResult> CreateUserAsync(Models.User user, string password);
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}
