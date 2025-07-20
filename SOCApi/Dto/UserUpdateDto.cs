namespace SOCApi.Dto
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActivated { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSuspended { get; set; }
    }
}