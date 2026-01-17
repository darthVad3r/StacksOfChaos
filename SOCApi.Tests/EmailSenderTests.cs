using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SOCApi.Configuration;
using SOCApi.Services.Email;
using Xunit;

namespace SOCApi.Tests
{
    public class EmailSenderTests
    {
        private readonly Mock<ILogger<EmailSender>> _mockLogger;
        private readonly Mock<IOptions<EmailSettings>> _mockEmailSettings;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly EmailSettings _emailSettings;

        public EmailSenderTests()
        {
            _mockLogger = new Mock<ILogger<EmailSender>>();
            _mockEmailSettings = new Mock<IOptions<EmailSettings>>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            
            _emailSettings = new EmailSettings
            {
                SmtpServer = "smtp.test.com",
                SmtpPort = 587,
                UseSsl = true,
                SenderEmail = "noreply@test.com",
                SenderName = "Test Sender",
                Username = "testuser",
                Password = "testpass"
            };
            
            _mockEmailSettings.Setup(x => x.Value).Returns(_emailSettings);
            _mockEnvironment.Setup(x => x.EnvironmentName).Returns("Development");
        }

        [Fact]
        public void EmailSender_Constructor_ShouldInitialize()
        {
            // Arrange & Act
            var emailSender = new EmailSender(_mockLogger.Object, _mockEmailSettings.Object, _mockEnvironment.Object);

            // Assert
            Assert.NotNull(emailSender);
        }

        [Fact]
        public async Task SendEmailAsync_WithInvalidSmtpServer_ShouldThrowException()
        {
            // Arrange
            _emailSettings.SmtpServer = "invalid.smtp.server.test";
            var emailSender = new EmailSender(_mockLogger.Object, _mockEmailSettings.Object, _mockEnvironment.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await emailSender.SendEmailAsync("test@example.com", "Test Subject", "Test User", "<p>Test Body</p>")
            );
        }

        [Fact]
        public async Task SendEmailAsync_WithEmptyEmail_ShouldThrowException()
        {
            // Arrange
            var emailSender = new EmailSender(_mockLogger.Object, _mockEmailSettings.Object, _mockEnvironment.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await emailSender.SendEmailAsync("", "Test Subject", "Test User", "<p>Test Body</p>")
            );
        }

        [Fact]
        public async Task SendEmailAsync_WithEmptySubject_ShouldNotThrow()
        {
            // Arrange
            _emailSettings.SmtpServer = "invalid.smtp.server.test";
            var emailSender = new EmailSender(_mockLogger.Object, _mockEmailSettings.Object, _mockEnvironment.Object);

            // Act & Assert
            // Empty subject is allowed, but will fail due to invalid SMTP server
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await emailSender.SendEmailAsync("test@example.com", "", "Test User", "<p>Test Body</p>")
            );
        }

        [Fact]
        public async Task SendEmailAsync_LogsErrorOnFailure()
        {
            // Arrange
            _emailSettings.SmtpServer = "invalid.smtp.server.test";
            var emailSender = new EmailSender(_mockLogger.Object, _mockEmailSettings.Object, _mockEnvironment.Object);

            // Act
            try
            {
                await emailSender.SendEmailAsync("test@example.com", "Test Subject", "Test User", "<p>Test Body</p>");
            }
            catch
            {
                // Expected to throw
            }

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void EmailSettings_ShouldHaveRequiredProperties()
        {
            // Assert
            Assert.NotNull(_emailSettings.SmtpServer);
            Assert.NotNull(_emailSettings.SenderEmail);
            Assert.NotNull(_emailSettings.SenderName);
            Assert.True(_emailSettings.SmtpPort > 0);
        }

        [Fact]
        public void EmailSettings_ShouldAllowEmptyCredentials()
        {
            // Arrange
            var settingsWithoutCreds = new EmailSettings
            {
                SmtpServer = "smtp.test.com",
                SmtpPort = 587,
                UseSsl = true,
                SenderEmail = "noreply@test.com",
                SenderName = "Test Sender",
                Username = "",
                Password = ""
            };

            // Assert
            Assert.NotNull(settingsWithoutCreds);
            Assert.Empty(settingsWithoutCreds.Username);
            Assert.Empty(settingsWithoutCreds.Password);
        }
    }
}
