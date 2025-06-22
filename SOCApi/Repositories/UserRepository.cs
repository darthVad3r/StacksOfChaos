using Microsoft.Data.SqlClient;
using SOCApi.Models;

namespace SOCApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = "Server=localhost;Database=SOCData;Integrated Security=True;TrustServerCertificate=True;";
        }

        public async Task<User> RegisterOrGetUserAsync(string email, string name, string password)
        {
            try
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
                    sqlCommand.Parameters.AddWithValue("@Password", password);

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
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {nameof(RegisterOrGetUserAsync)}: {ex.Message}");
                return null;
            }
        }

        // Implement other methods similarly...
    }
}