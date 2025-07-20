namespace SOCApi.Dto
{
    public class UserCreatedResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}