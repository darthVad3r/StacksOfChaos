using  SOCApi.Controllers;
using SOCApi.Models;


namespace SOCApi.Services
{
    public interface UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IAccountController _accountController;
        public UserService(ILogger<UserService> logger, IAccountController accountController)
        {
            _logger = logger;
            _accountController = accountController;
        }
        public async Task<User> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                // Check if the user already exists
                var user = await _accountController.GetUserByUsernameAsync(registerRequest.Username);
                if (user != null)
                {
                    return null;
                }
                // Create a new user
                user = new User
                {
                    Username = registerRequest.Username,
                    Email = registerRequest.Email,
                    Password = registerRequest.Password,
                    CreatedAt = DateTime.UtcNow
                };
                // Save the user to the database
                // _dbContext.Users.Add(user);
                // await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration");
                return null;
            }
        }
    }
}
