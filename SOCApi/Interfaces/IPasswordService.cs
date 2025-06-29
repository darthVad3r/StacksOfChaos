namespace SOCApi.Interfaces
{
    public interface IPasswordService
    {
        public void ChangePassword(string userName, string oldPassword, string newPassword);
        public void ResetPassword(string userName, string newPassword);
        public void UpdateUserPassword(string userName, string newPassword);
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hashedPassword);
    }
}
