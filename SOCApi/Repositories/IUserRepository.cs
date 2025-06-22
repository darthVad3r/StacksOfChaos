using SOCApi.Models;

namespace SOCApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> RegisterOrGetUserAsync(string email, string name, string password);
    }
}