using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SOCApi.Interfaces;
using SOCApi.Models;

namespace SOCApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(IConfiguration config, ILogger<AuthorizationService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public bool IsUserAuthenticated(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool IsUserAuthorized(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var authorizedUser = await ValidateUser(username, password);

            if (authorizedUser == null)
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", username);
                return null;
            }

            var jwtToken = GenerateJwtToken(authorizedUser);

            return new AuthResponse { Token = jwtToken, ExpiresAt = DateTime.UtcNow.AddHours(1) };
        }

        public async Task<IdentityUser> ValidateUser(string username, string password)
        {
            try
            {
                if (username == "testuser" && password == "password123")
                {
                    return await Task.FromResult(new IdentityUser { UserName = username, Email = $"{username}@example.com" });
                }

                return await Task.FromResult<IdentityUser>(null);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error validating user {Username}", username);
                throw new InvalidOperationException("An error occurred while validating the user.", ex);
            }
        }

        public string GenerateJwtToken(IdentityUser user)
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
