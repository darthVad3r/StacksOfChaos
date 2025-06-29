using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SOCApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SOCApi.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthController(IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _config = config;

            string getConnectionString = Common.GetConnectionString();
            _connectionString = getConnectionString;
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
        public async Task<IActionResult> RegisterOrGetUser(string signInCredentials)
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

            var user = JsonSerializer.Deserialize<User>(signInCredentials);

            var email = user.Email;
            var password = user.Password;

            var loggedInUser = GetUserByEmailAsync(email, password);

            var loggedInUserJson = JsonSerializer.Serialize(loggedInUser);

            // Assuming RegisterOrGetUserAsync returns the user ID
            // If it returns a User object, you might need to extract the ID from it
            return Ok(loggedInUserJson);
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
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] User userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input data" });
                }

                var user = await _userManager.FindByEmailAsync(userModel.Email);
                if (userModel == null || !await _userManager.CheckPasswordAsync(user, userModel.Password))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var token = GenerateJwtToken(user);

                return Ok(new { token });
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

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }   

    }
}
