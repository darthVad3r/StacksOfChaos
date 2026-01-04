using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks; // Add this using
using Scalar.AspNetCore;
using SOCApi.Configuration;
using SOCApi.Data;
using SOCApi.Models;
using SOCApi.Services;
using SOCApi.Services.Password;
using SOCApi.Services.User;
using SOCApi.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins(
            "https://localhost:4200",  // Angular dev
            "https://localhost:5001"   // TODO Add production URLs
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Database Context
builder.Services.AddDbContext<SocApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Health Checks
//builder.Services.AddHealthChecks()
//    .AddDbContext<SocApiDbContext>(); // Use AddDbContext instead of AddDbContextCheck

// Identity Configuration
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password requirements (OWASP guidelines)
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
    
    // Email confirmation
    options.SignIn.RequireConfirmedEmail = !builder.Environment.IsDevelopment();
    options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
})
.AddEntityFrameworkStores<SocApiDbContext>()
.AddDefaultTokenProviders()
.AddApiEndpoints();

// Application Services (alphabetically organized)
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IPasswordManagementService, PasswordManagementService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IPasswordValidationService, PasswordValidationService>();
builder.Services.AddScoped<IUserRetrievalService, UserRetrievalService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configuration
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

var app = builder.Build();

// Database initialization
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<SocApiDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred while migrating the database.");
    }
}

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<User>();
app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();