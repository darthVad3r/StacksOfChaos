using Microsoft.AspNetCore.Identity;
using SOCApi.Models;

namespace SOCApi.Interfaces
{
    public interface IAuthorizationService
    {
        public Task<string> LoginAsync(string username, string password);
        public void Logout();
        public bool IsUserAuthenticated(string username, string password);
        public bool IsUserAuthorized(string username, string password);
        public Task<IdentityUser> ValidateUser(string username, string password);
    }
}
