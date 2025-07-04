using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SOCApi.Interfaces;
using SOCApi.Models;

namespace SOCApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        public bool IsUserAuthenticated(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool IsUserAuthorized(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityUser<IdentityUser>> ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
