using SOCApi.Models;
using SOCApi.ViewModels;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SOCApi.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<bool> CheckUserExistAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task<User> LogOutAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<User> NewUserRegistrationAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(registerRequest);
                var user = new User
                {
                    Username = registerRequest.Username,
                    Email = registerRequest.Email,
                    Password = HashPassword(registerRequest.Password)
                };

                await AddNewUserToDatabaseAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during registration", ex);
            }
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public bool ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException(); ;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private async Task AddNewUserToDatabaseAsync(User user)
        {
            try
            {
                var query = "EXEC usp_AddNewUser @Username, @Email, @Password";
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar) { Value = user.Password });
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DataException("A database error occurred while adding a new user to the database", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("An invalid operation occurred while adding a new user to the database", ex);
            }
        }
    }
}
