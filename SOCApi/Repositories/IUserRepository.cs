using SOCApi.Models;

namespace SOCApi.Repositories
{
    public interface IUserRepository
    {
        Task GetUserByEmailAsync(string email);
        Task<User> RegisterOrGetUserAsync(string email, string name);
    }
}