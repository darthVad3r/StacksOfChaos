namespace SOCApi.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string recipientName, string htmlMessage);
    }
}