namespace SOCApi.Models.Requests
{
    public class EmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public EmailRequest() 
        { 
            Subject = string.Empty;
            Body = string.Empty;
            Email = string.Empty;
            RecipientName = string.Empty;
        }

    }
}
