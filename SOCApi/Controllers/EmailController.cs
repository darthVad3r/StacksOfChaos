using Microsoft.AspNetCore.Mvc;
using SOCApi.Services.Email;
using SOCApi.Models.Requests;

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

        await _emailService.SendEmailAsync(emailRequest.Email, emailRequest.RecipientName, emailRequest.Subject, emailRequest.Body);
        return Ok(new { Message = "Email sent successfully." });
    }
}