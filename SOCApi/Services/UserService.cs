using SOCApi.Models;
using SOCApi.ViewModels;

namespace SOCApi.Services
{
    public class UserService : IUserService
    {
        public Task<bool> CheckUserExistAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task<User> LogOutAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<User> NewUserRegistrationAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }

        public Task<User> RegisterAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();

        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException(); ;
        }

        
    }
}
