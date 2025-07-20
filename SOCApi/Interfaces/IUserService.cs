using SOCApi.Models;
using SOCApi.Dto;

namespace SOCApi.Interfaces
{
    public interface IUserService
    {
        public Task<UserCreatedResponse> RegisterNewUserAccountAsync(string newUserCredentials);
        public Task<bool> DeleteUserAsync(int userId);
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User?> GetUserByIdAsync(int userId);
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByEmailAsync(string email, string password);
        public Task<bool> IsUsernameUnique(string username);
        public Task<bool> UpdateUserAsync(User user);
        public Task<bool> UpdateUserPasswordAsync(int userId, string newPassword);
    }
}
