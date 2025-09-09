namespace SOCApi.Services.Password
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string userId, string newPassword);
        Task<bool> ValidatePasswordAsync(string password);
        Task<bool> IsPasswordStrongEnoughAsync(string password);
        Task<bool> IsPasswordPreviouslyUsedAsync(string userId, string password);
    }
}