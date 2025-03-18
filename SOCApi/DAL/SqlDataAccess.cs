using SOCApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SOCApi.DAL;
public class SqlDataAccess
{
    private readonly string _connectionString;
    private readonly ILogger<SqlDataAccess> _logger;

    public SqlDataAccess(IConfiguration configuration, ILogger<SqlDataAccess> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null or empty.");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User?> ValidateUser(string username, string password)
    {
        try
        {
            _logger.LogInformation("Validating user with username: {Username}", username);
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty.");
            }
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new SqlCommand("sp_ValidateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = password });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Role = reader.GetString(2)
                };
            }
            return null; // User not found or invalid credentials
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "An error occurred while validating user.");
            throw new InvalidOperationException("An error occurred while validating user.", sqlEx);
        }
    }
}
