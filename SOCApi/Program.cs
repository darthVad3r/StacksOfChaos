using Microsoft.OpenApi.Models;
using SOCApi.Services;
using SOCApi.Repositories;

namespace SOCApi;

public static class Program
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
        services.AddSingleton<TokenService>();

        // Application Services
        ConfigureApplicationServices(services, configuration);

        // HTTP Client
        ConfigureHttpClient(services);

        // CORS
        ConfigureCors(services, configuration);

        // Swagger
        ConfigureSwagger(services);
    }

    private static void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<BookSearchService>();
        // Add other application services here
        services.AddScoped<IUserRepository, UserRepository>(provider =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("DefaultConnection connection string is not configured.");
            return new UserRepository(connectionString);
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.Secure = CookieSecurePolicy.Always;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultSignInScheme = "Cookies";
        })
        .AddCookie("Cookies");
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
            ?? Common.GetAllowedOrigin();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder
                    .WithOrigins(corsOrigin)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
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
                    Email = "rmfinley@yahoo.com"
                }
            });
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            ConfigureDevelopmentMiddleware(app);
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigin");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void ConfigureDevelopmentMiddleware(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
