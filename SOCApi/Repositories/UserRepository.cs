using Microsoft.Data.SqlClient;
using SOCApi.Models;

namespace SOCApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _connectionString = "Server=localhost;Database=SOCData;Integrated Security=True;TrustServerCertificate=True;";
            _logger = logger;
        }

        public async Task<User> RegisterOrGetUserAsync(string email, string name, string password)
        {
            try
            {
                return await ExecuteGetOrCreateUserAsync(email, name, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {MethodName}", nameof(RegisterOrGetUserAsync));
                return null;
            }
        }

        private async Task<User> ExecuteGetOrCreateUserAsync(string email, string name, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = Common.StoredProcedures.GetOrCreateUser;
            using var sqlCommand = new SqlCommand(command, connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            sqlCommand.Parameters.AddWithValue("@Email", email);
            sqlCommand.Parameters.AddWithValue("@Name", name);
            sqlCommand.Parameters.AddWithValue("@Password", password);

            using var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Name = reader.GetString(2),
                    // Map other properties as needed
                };
            }
            return null;
        }

        private async Task<User> GetUserByEmailAsync(string email, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var command = Common.StoredProcedures.GetOrCreateUser;
                using var sqlCommand = new SqlCommand(command, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Email", email);
                sqlCommand.Parameters.AddWithValue("@Password", password);

                using var reader = await sqlCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Name = reader.GetString(2),
                        // Map other properties as needed
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {MethodName}", nameof(GetUserByEmailAsync));
                return null;
            }
        }

        // Implement other methods similarly...
    }
}