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
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUserService userService,
            ILogger<AccountController> logger)
        {
            _userService = userService;
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
