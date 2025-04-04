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

        public AuthController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                Console.WriteLine($"Testing connection string: {_connectionString}");
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                // Execute a simple query to verify the connection
                await using var cmd = new SqlCommand("SELECT 1", conn);
                var result = await cmd.ExecuteScalarAsync();

                if (result != null && result.ToString() == "1")
                {
                    return Ok(new { message = "Connection successful!" });
                }

                return StatusCode(500, new { error = "Connection test failed. Query did not return expected result." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection test failed: {ex.Message}");
                return StatusCode(500, new { error = "Connection test failed.", details = ex.Message });
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                // if (User.Identity.IsAuthenticated || User.Claims.Any(c => c.Type == ClaimTypes.Email))
                // {
                //     // User is already authenticated, redirect to home or return a message
                //     return Redirect("/api/auth/google-callback"); // Redirect to home or any other page
                // }
                // else if (User.Identity.IsAuthenticated)
                // {
                //     return BadRequest(new { error = "User already authenticated" });
                // }
                // Initiate Google authentication
                // Redirect to Google for authentication
                var props = new AuthenticationProperties
                {
                    RedirectUri = Common.GOOGLE_CALLBACK_URI,
                    Items = { { "scheme", GoogleDefaults.AuthenticationScheme } }
                };
                var redirectUrl = Url.Action("GoogleCallback", "Auth", null, Request.Scheme);
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                // Authenticate the Google response
                var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                {
                    return Unauthorized(new { error = "Google authentication failed" });
                }

                // Get user info from Google claims
                var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
                var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

                Console.WriteLine($"Email: {email}, Name: {name}");

                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Email claim not found in Google response.");
                    return BadRequest(new { error = "Unable to retrieve email from Google" });
                }

                // Register or get user from database
                var userId = await RegisterOrGetUser(email, name);

                // Generate JWT token
                var token = GenerateJwtToken(userId, email);

                // For API clients, return the token in the response
                // For redirect scenarios, you could redirect with the token as a query parameter
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if needed
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        private async Task<int> RegisterOrGetUser(string email, string name)
        {
            Console.WriteLine(_connectionString);
            await using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(Common.StoredProcedureGetOrCreateUser, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Name", name ?? "Unknown");
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("The stored procedure did not return a valid user ID. Ensure the database logic is correct.");
            }
            if (!int.TryParse(result?.ToString(), out var userId))
            {
                throw new InvalidOperationException("The user ID returned from the database is not a valid integer.");
            }
            return userId;
        }

        private string GenerateJwtToken(int userId, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
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
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
