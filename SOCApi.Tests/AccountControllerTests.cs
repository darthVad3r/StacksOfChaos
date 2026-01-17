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
    /// <summary>
    /// Unit tests for AccountController.
    /// Tests user registration, email confirmation, and account management.
    /// </summary>
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
            _mockUserManager = CreateMockUserManager();

            _controller = new AccountController(
                _mockUserService.Object,
                _mockUserManager.Object,
                _mockLogger.Object);
        }

        #region Register - Success Scenarios

        [Fact]
        public async Task Register_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = CreateValidRegisterRequest();
            var user = CreateUserFromRequest(request);
            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, request.FirstName, request.LastName))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Register_WithValidRequest_ReturnsUserObject()
        {
            // Arrange
            var request = CreateValidRegisterRequest();
            var expectedUser = CreateUserFromRequest(request);
            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, request.FirstName, request.LastName))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(expectedUser.Email, returnedUser.Email);
        }

        [Fact]
        public async Task Register_WithValidRequest_CallsUserService()
        {
            // Arrange
            var request = CreateValidRegisterRequest();
            var user = CreateUserFromRequest(request);
            _mockUserService.Setup(x => x.RegisterRequest(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            await _controller.Register(request);

            // Assert
            _mockUserService.Verify(x => x.RegisterRequest(
                request.Email, request.Password, request.FirstName, request.LastName),
                Times.Once);
        }

        #endregion

        #region Register - Error Scenarios

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
            var request = CreateValidRegisterRequest();

            _mockUserService.Setup(x => x.RegisterRequest(
                request.Email, request.Password, request.FirstName, request.LastName))
                .ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task Register_WithEmptyEmail_ReturnsBadRequest(string? email)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = email ?? "",
                Password = "Test123!@#",
                ConfirmPassword = "Test123!@#",
                FirstName = "Test",
                LastName = "User"
            };
            _mockUserService.Setup(x => x.RegisterRequest(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Email required"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Test123!@#",
                ConfirmPassword = "Different123!@#",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithServiceException_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRegisterRequest();
            _mockUserService.Setup(x => x.RegisterRequest(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Confirm Email - Success Scenarios

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsOk()
        {
            // Arrange
            var userId = "123";
            var token = "valid-token";
            var user = CreateSampleUser(userId);

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
        public async Task ConfirmEmail_WithValidToken_CallsUserManager()
        {
            // Arrange
            var userId = "123";
            var token = "valid-token";
            var user = CreateSampleUser(userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _controller.ConfirmEmail(userId, token);

            // Assert
            _mockUserManager.Verify(x => x.ConfirmEmailAsync(user, token), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsSuccessMessage()
        {
            // Arrange
            var userId = "123";
            var token = "valid-token";
            var user = CreateSampleUser(userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response.Message);
        }

        #endregion

        #region Confirm Email - Error Scenarios

        [Fact]
        public async Task ConfirmEmail_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var userId = "123";
            var token = "invalid-token";
            var user = CreateSampleUser(userId);

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

        [Theory]
        [InlineData("", "token")]
        [InlineData("userId", "")]
        [InlineData("", "")]
        public async Task ConfirmEmail_WithEmptyParameters_ReturnsBadRequest(string userId, string token)
        {
            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WhenUserManagerThrows_ReturnsBadRequest()
        {
            // Arrange
            var userId = "123";
            var token = "token";

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Controller Initialization Tests

        [Fact]
        public void AccountController_WithValidDependencies_Initializes()
        {
            // Act
            var controller = new AccountController(
                _mockUserService.Object,
                _mockUserManager.Object,
                _mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void AccountController_WithNullUserService_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AccountController(null!, _mockUserManager.Object, _mockLogger.Object));
        }

        [Fact]
        public void AccountController_WithNullUserManager_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AccountController(_mockUserService.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void AccountController_WithNullLogger_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AccountController(_mockUserService.Object, _mockUserManager.Object, null!));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a mock UserManager for testing.
        /// </summary>
        private static Mock<UserManager<User>> CreateMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        /// <summary>
        /// Creates a valid registration request for testing.
        /// </summary>
        private static RegisterRequest CreateValidRegisterRequest()
        {
            return new RegisterRequest
            {
                Email = "test@example.com",
                Password = "Test123!@#",
                ConfirmPassword = "Test123!@#",
                FirstName = "Test",
                LastName = "User"
            };
        }

        /// <summary>
        /// Creates a user from a registration request.
        /// </summary>
        private static User CreateUserFromRequest(RegisterRequest request)
        {
            return new User
            {
                Id = "123",
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a sample user for testing.
        /// </summary>
        private static User CreateSampleUser(string userId)
        {
            return new User
            {
                Id = userId,
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };
        }

        #endregion
    }
}
