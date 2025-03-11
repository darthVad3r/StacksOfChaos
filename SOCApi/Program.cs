using Microsoft.OpenApi.Models;
using SOCApi.Services;

namespace SOCApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // API Services
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // Application Services
        ConfigureApplicationServices(services);

        // HTTP Client
        ConfigureHttpClient(services);

        // CORS
        ConfigureCors(services, configuration);

        // Swagger
        ConfigureSwagger(services);
    }

    private static void ConfigureApplicationServices(IServiceCollection services)
    {
        services.AddScoped<BookSearchService>();
        // Add other application services here
    }

    private static void ConfigureHttpClient(IServiceCollection services)
    {
        services.AddHttpClient("BookClient", client =>
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var corsOrigin = configuration.GetValue<string>("CorsSettings:AllowedOrigin")
            ?? "http://localhost:52454";

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder
                    .WithOrigins(corsOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SOC API",
                Version = "v1",
                Description = "API for Stacks of Chaos",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "support@example.com"
                }
            });
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        // Development specific middleware
        if (app.Environment.IsDevelopment())
        {
            ConfigureDevelopmentMiddleware(app);
        }

        // Production middleware
        ConfigureProductionMiddleware(app);
    }

    private static void ConfigureDevelopmentMiddleware(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    private static void ConfigureProductionMiddleware(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigin");
        app.UseAuthorization();

        if (app is WebApplication webApp)
        {
            webApp.MapControllers();
        }
    }
}
