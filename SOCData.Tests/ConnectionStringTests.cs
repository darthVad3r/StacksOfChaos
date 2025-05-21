using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.Json;
using Xunit;
using SOCData;
using Moq;

namespace SOCData.Tests
{
    public class ConnectionStringTests
    {
        private readonly Mock<IHostEnvironment> _hostEnvironment;
        private readonly Mock<IConfiguration> _configuration;

        public ConnectionStringTests()
        {
            _hostEnvironment = new Mock<IHostEnvironment>();
            _hostEnvironment.Setup(env => env.EnvironmentName).Returns("Development");
            _configuration = new Mock<IConfiguration>();
            //_configuration.Setup(config => config.GetConnectionString("DefaultConnection"))
            //    .Returns("Server=localhost;Database=SOCData;User Id=sa;Password=your_password;");
        }


        [Fact]
        public void DefaultConnectionString_CanConnectTo_SOCData()
        {
            // Arrange
            //var connectionString = _configuration.Object.GetConnectionString("DefaultConnection");
            var connectionString = "Server=localhost;Database=SOCData;Integrated Security=True;TrustServerCertificate=True;";

            using var connection = new SqlConnection(connectionString);
            // Act
            connection.Open();
            // Assert
            Assert.Equal(System.Data.ConnectionState.Open, connection.State);
        }
    }
}
