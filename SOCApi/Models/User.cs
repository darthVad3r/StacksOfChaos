using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using SOCApi.Data;

namespace SOCApi.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActivated { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsSuspended { get; set; }
        public List<Spot> Spots { get; set; } = new List<Spot>();
        public string? Username { get; internal set; }
    }


public static class UserEndpoints
{
	public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(User));

        group.MapGet("/", async (SOCApiContext db) =>
        {
            return await db.Users.ToListAsync();
        })
        .WithName("GetAllUsers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<User>, NotFound>> (int id, SOCApiContext db) =>
        {
            return await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is User model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetUserById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, User user, SOCApiContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Name, user.Name)
                  .SetProperty(m => m.Token, user.Token)
                  .SetProperty(m => m.Role, user.Role)
                  .SetProperty(m => m.RefreshToken, user.RefreshToken)
                  .SetProperty(m => m.RefreshTokenExpiryTime, user.RefreshTokenExpiryTime)
                  .SetProperty(m => m.CreatedAt, user.CreatedAt)
                  .SetProperty(m => m.UpdatedAt, user.UpdatedAt)
                  .SetProperty(m => m.IsDeleted, user.IsDeleted)
                  .SetProperty(m => m.IsVerified, user.IsVerified)
                  .SetProperty(m => m.IsLocked, user.IsLocked)
                  .SetProperty(m => m.IsActivated, user.IsActivated)
                  .SetProperty(m => m.IsLoggedIn, user.IsLoggedIn)
                  .SetProperty(m => m.IsBlocked, user.IsBlocked)
                  .SetProperty(m => m.IsSuspended, user.IsSuspended)
                  .SetProperty(m => m.Id, user.Id)
                  .SetProperty(m => m.UserName, user.UserName)
                  .SetProperty(m => m.NormalizedUserName, user.NormalizedUserName)
                  .SetProperty(m => m.Email, user.Email)
                  .SetProperty(m => m.NormalizedEmail, user.NormalizedEmail)
                  .SetProperty(m => m.EmailConfirmed, user.EmailConfirmed)
                  .SetProperty(m => m.PasswordHash, user.PasswordHash)
                  .SetProperty(m => m.SecurityStamp, user.SecurityStamp)
                  .SetProperty(m => m.ConcurrencyStamp, user.ConcurrencyStamp)
                  .SetProperty(m => m.PhoneNumber, user.PhoneNumber)
                  .SetProperty(m => m.PhoneNumberConfirmed, user.PhoneNumberConfirmed)
                  .SetProperty(m => m.TwoFactorEnabled, user.TwoFactorEnabled)
                  .SetProperty(m => m.LockoutEnd, user.LockoutEnd)
                  .SetProperty(m => m.LockoutEnabled, user.LockoutEnabled)
                  .SetProperty(m => m.AccessFailedCount, user.AccessFailedCount)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        group.MapPost("/", async (User user, SOCApiContext db) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/User/{user.Id}",user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, SOCApiContext db) =>
        {
            var affected = await db.Users
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}}
