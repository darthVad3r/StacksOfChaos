using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using SOCApi.Models;
using SOCApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        /// Initiates Google authentication.
        /// </summary>
        /// <returns>
        /// Redirects the user to Google's authentication page if not already authenticated.
        /// </returns>
        /// <remarks>
        /// This method checks if the user is already authenticated. If not, it initiates the Google authentication process by redirecting the user to Google's login page. 
        /// Upon successful authentication, the user is redirected to the callback endpoint specified in the `RedirectUri`.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is already authenticated.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the redirect URI is null or empty.</exception>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var user = User;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    return BadRequest("User ID or email is missing.");
                }
                var token = GenerateJwtToken(userId, email);

                Console.WriteLine($"Generated token: {token}");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Failed to generate JWT token.");
                }
                var angularRedirectUrl = $"{Common.Endpoints.BASE_URL}/auth/callback?token={Uri.EscapeDataString(token)}";
                return Redirect(angularRedirectUrl);
            }
            {
                var userIdNew = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var emailNew = user.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userIdNew) || string.IsNullOrEmpty(emailNew))
                {
                    var authProperties = new AuthenticationProperties { RedirectUri = $"{Common.Endpoints.BASE_URL}/dashboard" };
                    return Challenge(authProperties, GoogleDefaults.AuthenticationScheme);
                }
                var tokenNew = GenerateJwtToken(userIdNew, emailNew);
                // Redirect to Angular callback route for token storage before dashboard guard
                var angularRedirectUrlNew = $"https://localhost:52454/auth/callback?token={Uri.EscapeDataString(tokenNew)}";
                var authPropertiesNew = new AuthenticationProperties { RedirectUri = angularRedirectUrlNew };
                return Challenge(authPropertiesNew, GoogleDefaults.AuthenticationScheme);
            }
        }

        [HttpGet("signin-google")]
        [AllowAnonymous]
        public IActionResult GoogleCallback()
        {
            var user = User; // This will contain the authenticated user's claims
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
            return Unauthorized("User is not authenticated.");
            }
            // Extract user information from claims
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
            return BadRequest("User ID or email is missing.");
            }
            // Generate JWT token
            var token = GenerateJwtToken(userId, email);
            if (string.IsNullOrEmpty(token))
            {
            return BadRequest("Failed to generate JWT token.");
            }
            // Redirect to Angular callback route for token storage before dashboard guard
            var angularRedirectUrl = $"https://localhost:52454/auth/callback?token={Uri.EscapeDataString(token)}";
            return Redirect(angularRedirectUrl);
        }

        /// <summary>
        /// Generates a JWT token for the user. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <returns>
        /// Returns the generated JWT token as a string.
        /// </returns>
        /// <remarks>
        /// This method creates a JWT token using the user's ID and email. The token is signed with a symmetric key and includes claims for the user's ID and email.
        /// The token is set to expire in 1 hour.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when userId or email is null or empty.</exception>
        /// <exception cref="SecurityTokenException">Thrown when there is an error with the token generation.</exception>
        [HttpPost("generate-token")]
        [Authorize]
        private string GenerateJwtToken(string userId, string email)
        {
            var key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            var base64Key = Convert.ToBase64String(key);
            var abc = Encoding.UTF8.GetBytes(base64Key);
            var symmetricKey = new SymmetricSecurityKey(abc);

            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email), // Use actual email
                new Claim("name", email ?? userId) // Optionally add a name claim if available
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        /// <summary>
        /// Logs the user out of their Google account.
        /// </summary>
        /// <returns>
        /// Redirects the user to the Google logout endpoint.
        /// </returns>
        /// <remarks>
        /// This method initiates the logout process for the user by redirecting them to the Google logout endpoint.
        /// </remarks>
        [HttpGet("google-logout")]
        public IActionResult GoogleLogout()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
