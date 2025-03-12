using SOCApi.Models;

namespace SOCApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequest registerRequest);
        Task<User> LoginAsync(LoginRequest loginRequest);
        Task<User> NewUserRegistrationAsync(RegisterRequest registerRequest);
        Task<User> LogOutAsync(string token);
        Task<User> UpdateUserAsync(User user);
    }
}
