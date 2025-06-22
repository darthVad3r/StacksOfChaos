using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using SOCApi.Models;
using SOCApi.Repositories;
using System.Data;
using System.Text.Json;

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
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(user);
                if (userData == null || !userData.TryGetValue("email", out var email) || !userData.TryGetValue("name", out var name) || !userData.TryGetValue("password", out var password))
                {
                    Console.WriteLine($"From {nameof(RegisterOrGetUser)}: Invalid user data received.");
                    return BadRequest("Invalid user data. Please provide a valid email, password and name.");
                }
                {
                    Console.WriteLine($"{nameof(RegisterOrGetUser)} called with email: {email}, name: {name}");
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                    {
                        return BadRequest("Email, password, and name are required.");
                    }

                    var registeredUser = await _userRepository.RegisterOrGetUserAsync(email, name, password);
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
                Console.WriteLine($"Error in {nameof(RegisterOrGetUser)}: {ex.Message}");
                // Log the exception (not implemented here)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        /// <summary>
        /// Sign in a user with email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Valid user object if sign-in is successful, or an error message if sign-in fails.
        /// </returns>
        /// <remarks>
        /// 
        /// </remarks>
        public async Task<IActionResult> SignIn(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return BadRequest("Email and password are required.");
                }

                var user = await GetUserByEmailAsync(email, password);

                if (user == null)
                {
                    return NotFound("User not found.");
                }
                // If user is found, proceed with sign-in logic
                // Convert user to a json object
                var stringUser = JsonSerializer.Serialize(user);

                // For example, you might want to generate a token or set a session
                return Ok("Sign-in successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {nameof(SignIn)}: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>
        /// 
        /// </returns>
        public async Task<User> GetUserByEmailAsync(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(Common.StoredProcedures.GetOrCreateUser, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);

            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),

                        // Map other properties as needed
                    };
                }
            }
            return null;
        }

    }
}
