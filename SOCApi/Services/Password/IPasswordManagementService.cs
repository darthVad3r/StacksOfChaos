namespace SOCApi.Services.Password
{
    /// <summary>
    /// Service responsible for password management operations
    /// </summary>
    public interface IPasswordManagementService
    {
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string userId, string newPassword);
        Task<bool> IsPasswordPreviouslyUsedAsync(string userId, string password);
    }
}