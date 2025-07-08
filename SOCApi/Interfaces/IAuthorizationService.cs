using Microsoft.AspNetCore.Identity;
using SOCApi.Models;

namespace SOCApi.Interfaces
{
    public interface IAuthorizationService
    {
        public Task<AuthResponse> LoginAsync(string username, string password);
        public bool IsUserAuthenticated(string username, string password);
        public bool IsUserAuthorized(string username, string password);
        public Task<IdentityUser> ValidateUser(string username, string password);
        public string GenerateJwtToken(IdentityUser user);
    }
}
