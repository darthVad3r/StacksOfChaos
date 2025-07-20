using Microsoft.OpenApi.Models;
using SOCApi.Interfaces;
using SOCApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOCApi.Data;
using SOCApi.Controllers;

app.MapUserEndpoints();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

namespace SOCApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<SOCApiContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SOCApiContext") ?? throw new InvalidOperationException("Connection string 'SOCApiContext' not found.")));

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
        // Add other application services here

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
        .AddCookie("Cookies", options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });

        services.AddAuthorization();

        // Register UserService with all required dependencies using a factory
        services.AddScoped<IUserService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<UserService>>();
            var emailService = provider.GetRequiredService<IEmailService>();
            var passwordService = provider.GetRequiredService<IPasswordService>();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "your-fallback-connection-string";
            return new UserService(logger, emailService, passwordService, connectionString);
        });

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IPermissionService, PermissionService>();
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
