using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SOCApi.Controllers;
using SOCApi.Models;
using SOCApi.Models.Requests;
using SOCApi.Services.User;
using Xunit;

namespace SOCApi.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            
            // Mock UserManager setup
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _controller = new AccountController(
                _mockUserService.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Register_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Test123!@#",
                ConfirmPassword = "Test123!@#",
                FirstName = "Test",
                LastName = "User"
            };

            var user = new User
            {
                Id = "123",
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, request.FirstName, request.LastName))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Register_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Test123!@#",
                ConfirmPassword = "Test123!@#"
            };

            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, null, null))
                .ThrowsAsync(new ArgumentException("Invalid input"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithExistingUser_ReturnsConflict()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "Test123!@#",
                ConfirmPassword = "Test123!@#"
            };

            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, null, null))
                .ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsOk()
        {
            // Arrange
            var userId = "123";
            var token = "valid-token";
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                UserName = "test@example.com"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var userId = "123";
            var token = "invalid-token";
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                UserName = "test@example.com"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WithNonExistentUser_ReturnsNotFound()
        {
            // Arrange
            var userId = "999";
            var token = "some-token";

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WithMissingParameters_ReturnsBadRequest()
        {
            // Act
            var result1 = await _controller.ConfirmEmail("", "token");
            var result2 = await _controller.ConfirmEmail("userId", "");
            var result3 = await _controller.ConfirmEmail("", "");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result1);
            Assert.IsType<BadRequestObjectResult>(result2);
            Assert.IsType<BadRequestObjectResult>(result3);
        }
    }
}
