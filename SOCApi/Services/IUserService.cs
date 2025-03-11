using SOCApi.Models;

namespace SOCApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequest registerRequest);
    }
}
