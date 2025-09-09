using Microsoft.AspNetCore.Identity;
using SOCApi.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SOCApi.Services.User;
using SOCApi.Configuration;
using SOCApi.Services;
using SOCApi.Services.Validation;
using SOCApi.Services.Password;
using SOCApi.Models;
using SOCApi.Services.Password;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add authentication for Identity API endpoints - Identity will handle bearer token setup
builder.Services.AddAuthentication();

// Password Services
builder.Services.AddScoped<IPasswordService, SOCApi.Services.Password.PasswordService>();
builder.Services.AddScoped<IPasswordValidationService, SOCApi.Services.Validation.PasswordValidationService>();
builder.Services.AddScoped<IPasswordHashingService, SOCApi.Services.Password.PasswordHashingService>();
//builder.Services.AddScoped<IPasswordPolicyService, SOCApi.Services.PasswordPolicy.PasswordPolicyService>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DbContext BEFORE Identity services
builder.Services.AddDbContext<SocApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    // options.UseInMemoryDatabase("SocApiDb");
    // // Explicitly configure for InMemory to avoid migration issues
    // options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    // // Disable migrations for InMemory database
    // options.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
});

// Configure Identity with proper options - use SOCApi.Models.User instead of IdentityUser
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password requirements (following OWASP guidelines)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;
    
    // User requirements
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Email confirmation (set to true in production)
    options.SignIn.RequireConfirmedEmail = false; // Set to true in production
    options.SignIn.RequireConfirmedAccount = false; // Set to true in production
})
    .AddEntityFrameworkStores<SocApiDbContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

// Register services AFTER Identity is configured
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordValidationService, PasswordValidationService>();
builder.Services.AddScoped<IUserRetrievalService, UserRetrievalService>();
builder.Services.AddScoped<IPasswordManagementService, PasswordManagementService>();

// Add missing password hashing service
builder.Services.AddScoped<SOCApi.Services.Password.IPasswordHashingService, SOCApi.Services.Password.PasswordHashingService>();


builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

var app = builder.Build();

// Ensure database is created for InMemory provider
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<SocApiDbContext>();
        // For InMemory databases, only ensure the database is created, don't run migrations
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        // Log the exception but don't fail startup for database initialization issues
        var logger = serviceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred while creating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // This provides the Swagger-like UI
}

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
