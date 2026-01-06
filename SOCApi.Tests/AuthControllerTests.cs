using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SOCApi.Controllers;
using Xunit;

namespace SOCApi.Tests
{
    /// <summary>
    /// Unit tests for AuthController.
    /// Tests login functionality and HTTP responses.
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockLogger.Object);
        }

        #region Login Endpoint Tests

        [Fact]
        public void Login_WhenCalled_ReturnsOkResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Login_WhenCalled_ReturnsExpectedMessage()
        {
            // Act
            var result = _controller.Login() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login successful", result.Value);
        }

        [Fact]
        public void Login_WhenCalled_LogsInformation()
        {
            // Act
            _controller.Login();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Login endpoint called")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Login_WhenCalled_StatusCodeIsOk()
        {
            // Act
            var result = _controller.Login() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        #endregion

        #region Controller Initialization Tests

        [Fact]
        public void AuthController_WithValidLogger_Initializes()
        {
            // Act
            var controller = new AuthController(_mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void AuthController_WithNullLogger_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AuthController(null!));
        }

        #endregion
    }
}
