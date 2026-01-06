using Microsoft.AspNetCore.Mvc;
using Moq;
using SOCApi.Controllers;
using SOCApi.Models.Requests;
using SOCApi.Services.Email;
using Xunit;

namespace SOCApi.Tests
{
    /// <summary>
    /// Unit tests for EmailController.
    /// Tests email sending functionality, error handling, and validation.
    /// </summary>
    public class EmailControllerTests
    {
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly EmailController _controller;

        public EmailControllerTests()
        {
            _mockEmailSender = new Mock<IEmailSender>();
            _controller = new EmailController(_mockEmailSender.Object);
        }

        #region Send Email - Success Scenarios

        [Fact]
        public async Task SendEmail_WithValidRequest_ReturnOk()
        {
            // Arrange
            var request = CreateValidEmailRequest();
            _mockEmailSender.Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WithValidRequest_CallsEmailService()
        {
            // Arrange
            var request = CreateValidEmailRequest();
            _mockEmailSender.Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.SendEmail(request);

            // Assert
            _mockEmailSender.Verify(x => x.SendEmailAsync(
                request.Email,
                request.Subject,
                request.RecipientName,
                request.Body),
                Times.Once);
        }

        [Fact]
        public async Task SendEmail_WithValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            var request = CreateValidEmailRequest();
            _mockEmailSender.Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response.Message);
            Assert.Contains("successfully", response.Message.ToString()!);
        }

        #endregion

        #region Send Email - Error Scenarios

        [Fact]
        public async Task SendEmail_WhenServiceThrowsInvalidOperation_Returns500()
        {
            // Arrange
            var request = CreateValidEmailRequest();
            var exception = new InvalidOperationException("SMTP error");
            _mockEmailSender.Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task SendEmail_WhenServiceThrowsException_ReturnsErrorMessage()
        {
            // Arrange
            var request = CreateValidEmailRequest();
            var errorMessage = "Email service unavailable";
            _mockEmailSender.Setup(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException(errorMessage));

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            var response = statusResult.Value as dynamic;
            Assert.Contains("Failed", response.Message.ToString()!);
            Assert.Contains(errorMessage, response.Error.ToString()!);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task SendEmail_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new EmailRequest { Email = "", Subject = "", Body = "" };
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SendEmail_WithNullRequest_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _controller.SendEmail(null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task SendEmail_WithInvalidEmail_ReturnsBadRequest(string? email)
        {
            // Arrange
            var request = new EmailRequest
            {
                Email = email ?? "",
                Subject = "Test",
                Body = "Test body",
                RecipientName = "Test"
            };
            _controller.ModelState.AddModelError("Email", "Invalid email");

            // Act
            var result = await _controller.SendEmail(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region Controller Initialization Tests

        [Fact]
        public void EmailController_WithValidDependencies_Initializes()
        {
            // Act
            var controller = new EmailController(_mockEmailSender.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void EmailController_WithNullEmailSender_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new EmailController(null!));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid email request for testing.
        /// </summary>
        private static EmailRequest CreateValidEmailRequest()
        {
            return new EmailRequest
            {
                Email = "test@example.com",
                Subject = "Test Subject",
                RecipientName = "Test User",
                Body = "Test email body content"
            };
        }

        #endregion
    }
}
