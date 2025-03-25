using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _config = config;
            _logger = logger;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Google login
        /// </summary>
        /// <remarks>
        /// This method initiates the Google login process by redirecting the user to the Google login page.
        /// </remarks>
        /// <returns>
        /// Redirect to Google login page
        /// </returns>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Common.GOOGLE_REDIRECT_URI
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Google Callback
        /// </summary>
        /// <remarks>
        /// This method handles the callback from Google after the user has authenticated.
        /// It retrieves the authentication information from the HttpContext, including the user's email and name.
        /// If authentication is successful, it registers or retrieves the user from the database.
        /// Finally, it generates a JWT token for the authenticated user and returns it in the response.
        /// </remarks>
        [HttpGet("google-callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GoogleCallback()
        {
            var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!info.Succeeded)
            {
                return BadRequest("Failed to authenticate with Google");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var userId = await RegisterOrGetUser(email, name);

            var token = GenerateJwtToken(userId, email);
            return Ok(new { token });
        }

        /// <summary>
        /// Register or get user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <remarks>
        /// This method attempts to register a new user or retrieve an existing user from the database.
        /// It uses a stored procedure named 'usp_GetOrCreateUser' to perform the operation.
        /// The method takes the user's email and name as parameters and returns the user's ID.
        /// </remarks>
        /// <returns>
        /// userId
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private async Task<int> RegisterOrGetUser(string email, string name)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand("usp_GetOrCreateUser", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Name", name);

            var userId = (int)await cmd.ExecuteScalarAsync();

            return userId;
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user
        /// </summary>
        /// <remarks>
        /// This method creates a JWT token using the user's ID and email. 
        /// The token is signed using the HMAC SHA256 algorithm and includes claims for the user's ID, email, and a unique identifier (JTI).
        /// The token is set to expire in 1 hour.
        /// </remarks>
        /// <returns>
        /// JWT token
        /// </returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private string GenerateJwtToken(int userId, string email)
        {
            try
            {
                _logger.LogInformation("Generating JWT token for user {userId}", userId);

                if (string.IsNullOrEmpty(_config["Jwt:Key"]) || string.IsNullOrEmpty(_config["Jwt:Issuer"]) || string.IsNullOrEmpty(_config["Jwt:Audience"]))
                {
                    _logger.LogError("JWT configuration is missing");
                    throw new Exception("JWT configuration is missing");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate JWT token");
                throw new Exception("Failed to generate JWT token", ex);
            }
        }

    }

}
