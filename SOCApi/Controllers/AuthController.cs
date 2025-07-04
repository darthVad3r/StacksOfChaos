using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SOCApi.Interfaces;
using SOCApi.Models;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, IAuthorizationService authorizationService, ILogger<AuthController> logger)
        {
            _config = config;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login model state.");
                return BadRequest(ModelState);
            }

            var token = await ExecuteLogin(userCredentials);

            if (token == null)
            {
                _logger.LogWarning("Login failed for user: {Username}", userCredentials.Username);
                return Unauthorized("Invalid username or password.");
            }

            return Ok(token);
        }

        private async Task<String?> ExecuteLogin(UserCredentials userCredentials)
        {
            var authorizedUser = await _authorizationService.ValidateUser(userCredentials.Username, userCredentials.Password);

            if (authorizedUser == null)
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", userCredentials.Username);
                return null;
            }

            var jwtToken = GenerateJwtToken(authorizedUser);

            return jwtToken;
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The IdentityUser for whom the token is generated.</param>
        /// <returns>A JWT token string representing the authenticated user.</returns>
        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, "User") // You can add roles or other claims as needed
            };

            var keyString = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                _logger.LogError("JWT configuration is missing or incomplete.");
                throw new InvalidOperationException("JWT configuration is not properly set.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}