namespace SOCApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActivated { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsLoggedOut { get; set; }
        public bool IsOnline { get; set; }
        public bool IsOffline { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsUnblocked { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsUnsuspended { get; set; }
        public List<Spot> Spots { get; set; } = new List<Spot>();

    }
}
