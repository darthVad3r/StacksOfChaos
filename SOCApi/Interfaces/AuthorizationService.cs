namespace SOCApi.Interfaces
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

        public void ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
