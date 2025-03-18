using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SOCApi.DAL;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly TokenService _tokenService;

        public AuthController(SqlDataAccess dataAccess, TokenService tokenService)
        {
            _dataAccess = dataAccess;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _dataAccess.ValidateUser(model.Username, model.Password);
            if (user == null) return Unauthorized();

            var token = _tokenService.GenerateToken(user.Username, user.Role);
            return Ok(new { Token = token });
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
