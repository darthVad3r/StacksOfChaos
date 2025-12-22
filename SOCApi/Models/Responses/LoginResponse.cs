namespace SOCApi.Models.Responses
{
    public class LoginResponse
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public required string Message { get; set; }
        public string? FullName { get; set; }
    }
}
