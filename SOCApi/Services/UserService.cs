using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SOCApi.Models;
using SOCApi.ViewModels;
using System.Data;

namespace SOCApi.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;
        public UserService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public Task<User> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
        public Task<User> LogOutAsync(string token)
        {
            throw new NotImplementedException();
        }
        public async Task<User> RegisterUserAsync(string request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);

                var registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(request);

                if (registerRequest == null)
                {
                    ArgumentNullException.ThrowIfNull(registerRequest, nameof(request));
                    throw new ArgumentException("Passwords do not match");
                }

                // Hash the password
                //var hashedPassword = HashPassword(registerRequest.Password);

                // Check if the user already exists
                var userExists = await CheckUserExistAsync(registerRequest.Email);

                //var user = new User
                //{
                //    Username = registerRequest.Username,
                //    Email = registerRequest.Email,
                //    Password = hashedPassword
                //};

                //var newUser = await AddNewUserToDatabaseAsync(user);

                //await AddNewUserToDatabaseAsync(user);

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during registration: {ex.Message}", ex);
            }
        }
        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
        public Task<User> DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }
        public Task<bool> CheckUserExistAsync(string email)
        {
            throw new NotImplementedException();
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
                var hash = System.Security.Cryptography.SHA256.HashData(bytes);
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
        public Task<User> RegisterAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }
        public Task<string> GenerateJwtToken(User user)
        {
            throw new NotImplementedException();
        }
        public Task<bool> ValidateCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }
        Task<string> IUserService.HashPassword(string password)
        {
            throw new NotImplementedException();
        }
        Task IUserService.AddNewUserToDatabaseAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
