using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SOCApi.Data;

namespace SOCApi.Tests.Integration
{
    /// <summary>
    /// Custom WebApplicationFactory for integration testing.
    /// Configures an in-memory database and disables unnecessary services.
    /// Follows SOLID principles with single responsibility for test configuration.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove existing database-related services
                RemoveDbContextServices(services);

                // Add in-memory database for testing
                services.AddDbContext<SocApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    options.EnableSensitiveDataLogging();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                });

                // Ensure health checks are registered
                if (!services.Any(s => s.ServiceType == typeof(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService)))
                {
                    services.AddHealthChecks();
                }
            });
        }

        /// <summary>
        /// Removes DbContext-related services to avoid conflicts between SQL Server and InMemory providers.
        /// Follows DRY principle by extracting this logic to a separate method.
        /// </summary>
        private static void RemoveDbContextServices(IServiceCollection services)
        {
            // Remove DbContextOptions
            var dbContextOptionsDescriptor = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<SocApiDbContext>))
                .ToList();
            foreach (var descriptor in dbContextOptionsDescriptor)
            {
                services.Remove(descriptor);
            }

            // Remove DbContext
            var dbContextDescriptor = services
                .Where(d => d.ServiceType == typeof(SocApiDbContext))
                .ToList();
            foreach (var descriptor in dbContextDescriptor)
            {
                services.Remove(descriptor);
            }
        }
    }
}
