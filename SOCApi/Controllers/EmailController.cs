
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SOCApi.Models;
using SOCApi.Services.Email;
using Microsoft.AspNetCore.Authorization;

namespace SOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public EmailController(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }
        // Add action methods here
        
        // TODO: Implement SendEmail endpoint with proper EmailRequest DTO
        /*
        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _emailService.SendEmailAsync(emailRequest);
            if (!result) 
            {
                return StatusCode(500, "Failed to send email");
            }
            
            return Ok("Email sent successfully");
        }
        */
    }
}