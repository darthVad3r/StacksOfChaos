using SOCApi.Models;
using SOCApi.ViewModels;

namespace SOCApi.Services
{
    public interface IUserService
    {
        // Existing method signatures
        Task<User> LoginAsync(LoginRequest loginRequest);
        Task<User> LogOutAsync(string token);
        Task<User> RegisterUserAsync(string request);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(User user);
        Task<bool> CheckUserExistAsync(string email);
        Task<bool> ValidateCredentials(string username, string password);
        Task<string> HashPassword(string password);

        // New method signature
        Task AddNewUserToDatabaseAsync(User user);
    }
}
