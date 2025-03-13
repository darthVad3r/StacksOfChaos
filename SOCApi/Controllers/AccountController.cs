using Microsoft.AspNetCore.Mvc;
using SOCApi.Services;
using SOCApi.ViewModels;
using SOCApi.Models;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase, IAccountController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.RegisterAsync(new RegisterRequest
                { Username = request.Username, Email = request.Email, Password = request.Password });

                var response = new RegisterResponse
                {
                    //Id = user.Id,
                    //Username = user.Username,
                    //Email = user.Email,
                    //CreatedAt = user.CreatedAt
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration");
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }
    }
}