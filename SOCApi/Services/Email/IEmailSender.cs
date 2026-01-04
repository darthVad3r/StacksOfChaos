namespace SOCApi.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string RecipientName, string htmlMessage);
    }
}