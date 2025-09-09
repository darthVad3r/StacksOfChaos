using BCrypt.Net;

namespace SOCApi.Services.Password
{
    /// <summary>
    /// Service responsible for password hashing and verification operations
    /// </summary>
    public interface IPasswordHashingService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateRandomPassword(int length = 12);

        Task<string> GeneratePasswordResetTokenAsync();
        Task<bool> ValidatePasswordResetTokenAsync(string token, string userId);
        Task InvalidatePasswordResetTokenAsync(string token);
    }
}