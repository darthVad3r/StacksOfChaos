using Org.BouncyCastle.Crypto.Generators;
using SOCApi.Interfaces;

namespace SOCApi.Services
{
    public class PasswordService : IPasswordService
    {
        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public string HashPassword(string password)
        {
            var passwordBytes = BCrypt.PasswordToByteArray(password.ToCharArray());
            var salt = new byte[16]; // Example salt, replace with actual salt generation logic
            var hashedPasswordBytes = BCrypt.Generate(passwordBytes, salt, 12); // Example cost factor
            var hashedPassword = Convert.ToBase64String(hashedPasswordBytes);

            return hashedPassword;
        }

        public void ResetPassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
