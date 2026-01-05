using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SOCApi.Models;
using SOCApi.Models.Requests;
using SOCApi.Models.Responses;
using SOCApi.Services.User;

namespace SOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUserService userService,
            UserManager<User> userManager,
            ILogger<AccountController> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Delegate business logic to service
                var user = await _userService.RegisterRequest(request.Email, request.Password, request.FirstName, request.LastName);

                _logger.LogInformation("User {Email} registered successfully via API", request.Email);

                return Ok(CreateSuccessResponse(user));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Registration validation failed for {Email}", request.Email);
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration business logic failed for {Email}", request.Email);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "User ID and token are required" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Email confirmed for user {Email}", user.Email);
                    return Ok(new { message = "Email confirmed successfully. You can now log in." });
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for user {Email}: {Errors}", user.Email, errors);
                return BadRequest(new { message = "Email confirmation failed", errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during email confirmation");
                return StatusCode(500, new { message = "An error occurred during email confirmation" });
            }
        }

        private static RegisterResponse CreateSuccessResponse(IdentityUser user)
        {
            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Message = "Registration successful. Please check your email for confirmation."
            };
        }
    }
}
