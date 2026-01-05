using Microsoft.AspNetCore.Mvc;
using SOCApi.Services.Email;
using SOCApi.Models.Requests;

namespace SOCApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailService;

        public EmailController(IEmailSender emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _emailService.SendEmailAsync(
                    emailRequest.Email, 
                    emailRequest.Subject, 
                    emailRequest.RecipientName, 
                    emailRequest.Body);
                return Ok(new { Message = "Email sent successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { Message = "Failed to send email.", Error = ex.Message });
            }
        }
    }
}