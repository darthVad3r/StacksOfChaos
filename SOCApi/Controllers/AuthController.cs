using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SOCApi.Repositories;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly string _connectionString;
        private readonly IUserRepository _userRepository;

        public AuthController(IConfiguration config, IUrlHelperFactory urlHelperFactory, IUserRepository userRepository)
        {
            _config = config;
            _urlHelperFactory = urlHelperFactory;
            _userRepository = userRepository;

            //_connectionString = _config.GetConnectionString("DefaultConnection");
            _connectionString = "Server=localhost;Database=SOCData;Integrated Security=True;TrustServerCertificate=True;";
        }

        /// <summary>
        /// Registers a new user or retrieves an existing user from the database.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="name">The name of the user.</param>
        /// <returns>
        /// Returns the user ID if registration is successful, or an error message if registration fails.
        /// </returns>
        /// <remarks>
        /// This method checks if a user with the specified email already exists in the database. If not, it registers a new user with the provided email and name.
        /// If the user already exists, it retrieves the existing user's ID. The user ID is then returned to the client.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when email or name is null or empty.</exception>
        [Authorize]
        [HttpPost("register-or-get-user")]
        [Route("register-or-get-user")]
        public async Task<IActionResult> RegisterOrGetUser(string email, string name)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                return BadRequest("Email and name are required.");
            }

            var user = await _userRepository.RegisterOrGetUserAsync(email, name);
            if (user == null)
            {
                return BadRequest("User registration failed.");
            }
            // Assuming RegisterOrGetUserAsync returns the user ID
            // If it returns a User object, you might need to extract the ID from it
            var userId = user.Id.ToString();
            return Ok(userId);
        }
    }
}
