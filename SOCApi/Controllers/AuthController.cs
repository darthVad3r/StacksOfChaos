using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace SOCApi.Controllers
{
    // [Authorize] // Temporarily commented for testing
    [ApiController]
    [Route("[controller]")]
    // [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")] // Temporarily commented for testing
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            _logger.LogInformation("Login endpoint called at {Time}", DateTime.UtcNow);
            return Ok("Login successful");
        }
    }
}