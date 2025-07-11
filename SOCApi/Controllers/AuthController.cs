using Microsoft.AspNetCore.Mvc;
using SOCApi.Interfaces;
using SOCApi.Models;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthorizationService authorizationService, ILogger<AuthController> logger)
        {
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

            var authResponse = await _authorizationService.LoginAsync(userCredentials.Username, userCredentials.Password);

            if (authResponse == null)
            {
                _logger.LogWarning("Login failed for user: {Username}", userCredentials.Username);
                return Unauthorized(new { error = "Invalid username or password."});
            }

            return Ok(authResponse);
        }
    }
}