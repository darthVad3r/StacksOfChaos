namespace SOCApi.Models.Responses
{
    public class RegisterResponse
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string Message { get; set; }
        public string? FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
