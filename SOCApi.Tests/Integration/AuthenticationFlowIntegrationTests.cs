using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SOCApi.Data;
using SOCApi.Models;
using SOCApi.Models.Requests;
using SOCApi.Models.Responses;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SOCApi.Tests.Integration
{
    /// <summary>
    /// Integration tests for the complete authentication flow.
    /// Tests user registration, login, email confirmation, and token refresh end-to-end.
    /// </summary>
    [Collection("Integration Tests")]
    public class AuthenticationFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private IServiceScope? _scope;
        private SocApiDbContext? _dbContext;

        public AuthenticationFlowIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<SocApiDbContext>();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.DisposeAsync();
            }
            _scope?.Dispose();
            _client.Dispose();
        }

        #region User Registration Tests

        [Fact]
        public async Task RegisterUser_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var registerRequest = CreateValidRegisterRequest("test1@example.com");

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", registerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
            Assert.NotNull(result);
            Assert.Equal(registerRequest.Email, result.Email);
            Assert.NotNull(result.UserId);
            Assert.NotEmpty(result.UserId);
        }

        [Fact]
        public async Task RegisterUser_WithValidData_StoresUserInDatabase()
        {
            // Arrange
            var registerRequest = CreateValidRegisterRequest("test2@example.com");

            // Act
            await _client.PostAsJsonAsync("/api/account/register", registerRequest);

            // Assert
            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);
            Assert.NotNull(user);
            Assert.Equal(registerRequest.Email, user.Email);
            Assert.Equal(registerRequest.FirstName, user.FirstName);
            Assert.Equal(registerRequest.LastName, user.LastName);
        }

        [Fact]
        public async Task RegisterUser_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var email = "duplicate@example.com";
            var firstRequest = CreateValidRegisterRequest(email);
            await _client.PostAsJsonAsync("/api/account/register", firstRequest);

            var secondRequest = CreateValidRegisterRequest(email);

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", secondRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Theory]
        [InlineData("", "Password123!", "Test", "User")]
        [InlineData("invalid-email", "Password123!", "Test", "User")]
        [InlineData("test@example.com", "weak", "Test", "User")]
        [InlineData("test@example.com", "Password123!", "", "User")]
        public async Task RegisterUser_WithInvalidData_ReturnsBadRequest(
            string email, string password, string firstName, string lastName)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = email,
                Password = password,
                ConfirmPassword = password,
                FirstName = firstName,
                LastName = lastName
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_WithMismatchedPasswords_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword123!",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Email Confirmation Tests

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsSuccess()
        {
            // Arrange
            var registerRequest = CreateValidRegisterRequest("confirm1@example.com");
            await _client.PostAsJsonAsync("/api/account/register", registerRequest);

            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);
            Assert.NotNull(user);

            // Generate a valid token (in real scenario, this would come from email)
            var userManager = _scope!.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Act
            var response = await _client.GetAsync($"/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // Verify email is confirmed in database
            var confirmedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(confirmedUser);
            Assert.True(confirmedUser.EmailConfirmed);
        }

        [Fact]
        public async Task ConfirmEmail_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = CreateValidRegisterRequest("confirm2@example.com");
            await _client.PostAsJsonAsync("/api/account/register", registerRequest);

            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);
            Assert.NotNull(user);

            var invalidToken = "invalid-token-12345";

            // Act
            var response = await _client.GetAsync($"/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(invalidToken)}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ConfirmEmail_WithMissingUserId_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/account/confirm-email?token=some-token");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ConfirmEmail_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid().ToString();
            var token = "some-token";

            // Act
            var response = await _client.GetAsync($"/api/account/confirm-email?userId={nonExistentUserId}&token={Uri.EscapeDataString(token)}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Complete Authentication Flow Tests

        [Fact]
        public async Task CompleteAuthFlow_RegisterAndConfirmEmail_WorksEndToEnd()
        {
            // Arrange
            var email = "fullflow@example.com";
            var registerRequest = CreateValidRegisterRequest(email);

            // Act 1: Register user
            var registerResponse = await _client.PostAsJsonAsync("/api/account/register", registerRequest);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
            Assert.NotNull(registerResult);
            Assert.NotNull(registerResult.UserId);

            // Verify user is not confirmed initially
            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == email);
            Assert.NotNull(user);
            Assert.False(user.EmailConfirmed);

            // Act 2: Confirm email
            var userManager = _scope!.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmResponse = await _client.GetAsync($"/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}");
            Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

            // Assert: Verify final state
            var confirmedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(confirmedUser);
            Assert.True(confirmedUser.EmailConfirmed);
            Assert.Equal(email, confirmedUser.Email);
        }

        [Fact]
        public async Task MultipleUsers_CanRegisterAndConfirmIndependently()
        {
            // Arrange
            var user1Email = "user1@example.com";
            var user2Email = "user2@example.com";

            // Act: Register both users
            var request1 = CreateValidRegisterRequest(user1Email);
            var request2 = CreateValidRegisterRequest(user2Email);

            var response1 = await _client.PostAsJsonAsync("/api/account/register", request1);
            var response2 = await _client.PostAsJsonAsync("/api/account/register", request2);

            // Assert: Both registrations succeed
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            // Verify both users exist in database
            var user1 = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == user1Email);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user2Email);

            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotEqual(user1.Id, user2.Id);

            // Confirm first user's email
            var userManager = _scope!.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
            var token1 = await userManager.GenerateEmailConfirmationTokenAsync(user1);

            var confirmResponse = await _client.GetAsync($"/api/account/confirm-email?userId={user1.Id}&token={Uri.EscapeDataString(token1)}");
            Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

            // Assert: User 1 confirmed, User 2 not confirmed
            var confirmedUser1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user1.Id);
            var unconfirmedUser2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user2.Id);

            Assert.True(confirmedUser1!.EmailConfirmed);
            Assert.False(unconfirmedUser2!.EmailConfirmed);
        }

        #endregion

        #region Security and Edge Case Tests

        [Fact]
        public async Task RegisterUser_PasswordNotStoredInPlainText()
        {
            // Arrange
            var password = "SecurePassword123!";
            var request = CreateValidRegisterRequest("security@example.com", password);

            // Act
            await _client.PostAsJsonAsync("/api/account/register", request);

            // Assert
            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            Assert.NotNull(user);
            
            // Password should be hashed, not plain text
            Assert.NotEqual(password, user.PasswordHash);
            Assert.NotNull(user.PasswordHash);
            Assert.NotEmpty(user.PasswordHash);
        }

        [Fact]
        public async Task RegisterUser_CreatesUserWithExpectedDefaults()
        {
            // Arrange
            var request = CreateValidRegisterRequest("defaults@example.com");

            // Act
            await _client.PostAsJsonAsync("/api/account/register", request);

            // Assert
            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            Assert.NotNull(user);
            Assert.False(user.EmailConfirmed);
            Assert.False(user.TwoFactorEnabled);
            Assert.NotNull(user.SecurityStamp);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
            Assert.True(user.CreatedAt >= DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task ConfirmEmail_CannotBeReusedAfterSuccess()
        {
            // Arrange
            var request = CreateValidRegisterRequest("reuse@example.com");
            await _client.PostAsJsonAsync("/api/account/register", request);

            var user = await _dbContext!.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userManager = _scope!.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);

            // Act: Use token once (should succeed)
            var firstResponse = await _client.GetAsync($"/api/account/confirm-email?userId={user!.Id}&token={Uri.EscapeDataString(token)}");
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            // Act: Try to use same token again (should fail)
            var secondResponse = await _client.GetAsync($"/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid registration request with standard test data.
        /// Follows DRY principle by centralizing test data creation.
        /// </summary>
        private static RegisterRequest CreateValidRegisterRequest(string email, string? password = null)
        {
            return new RegisterRequest
            {
                Email = email,
                Password = password ?? "TestPassword123!",
                ConfirmPassword = password ?? "TestPassword123!",
                FirstName = "Test",
                LastName = "User"
            };
        }

        #endregion
    }
}
