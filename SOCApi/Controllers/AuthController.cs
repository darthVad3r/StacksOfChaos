using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SOCApi.Repositories;

namespace SOCApi.Controllers
{
    [AutoValidateAntiforgeryToken]
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
        [HttpPost("register-or-get-user")]
        [Route("register-or-get-user")]
        public async Task<IActionResult> RegisterOrGetUser(string user)
        {
            try
            {
                // Parse the user parameter to extract email and name
                var userData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(user);
                if (userData == null || !userData.TryGetValue("email", out var email) || !userData.TryGetValue("name", out var name))
                {
                    return BadRequest("Invalid user data. Please provide both email and name.");
                }
                {
                    Console.WriteLine($"RegisterOrGetUser called with email: {email}, name: {name}");
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
                    {
                        return BadRequest("Email and name are required.");
                    }

                    var registeredUser = await _userRepository.RegisterOrGetUserAsync(email, name);
                    if (registeredUser == null)
                    {
                        return BadRequest("User registration failed.");
                    }
                    // Assuming RegisterOrGetUserAsync returns the user ID
                    // If it returns a User object, you might need to extract the ID from it
                    var userId = registeredUser.Id.ToString();
                    return Ok(userId);
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"ArgumentNullException in RegisterOrGetUser: {ex.Message}");
                return BadRequest("Email and name cannot be null or empty.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterOrGetUser: {ex.Message}");
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
