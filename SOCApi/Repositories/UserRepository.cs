using Microsoft.Data.SqlClient;
using SOCApi.Models;

namespace SOCApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> RegisterOrGetUserAsync(string email, string name)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = Common.StoredProcedures.GetOrCreateUser;
                using var sqlCommand = new SqlCommand(command, connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                sqlCommand.Parameters.AddWithValue("@Email", email);
                sqlCommand.Parameters.AddWithValue("@Name", name);

                using (var reader = await sqlCommand.ExecuteReaderAsync())
                {
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
                }
            }
            return null;
        }

        // Implement other methods similarly...
    }
}