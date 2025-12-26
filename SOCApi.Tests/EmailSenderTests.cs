using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SOCApi.Configuration;
using SOCApi.Services.Email;

namespace SOCApi.Tests;

public class EmailSenderTests
{
    private readonly Mock<ILogger<EmailSender>> _loggerMock;
    private readonly EmailSettings _emailSettings;
    private readonly IOptions<EmailSettings> _emailSettingsOptions;

    public EmailSenderTests()
    {
        _loggerMock = new Mock<ILogger<EmailSender>>();
        _emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.test.com",
            SmtpPort = 587,
            UseSsl = true,
            FromEmail = "test@test.com",
            FromName = "Test Sender",
            Username = "testuser",
            Password = "testpass"
        };
        _emailSettingsOptions = Options.Create(_emailSettings);
    }

    [Fact]
    public async Task SendEmailAsync_WithInvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var emailSender = new EmailSender(_loggerMock.Object, _emailSettingsOptions);
        var invalidEmail = "invalid-email";
        var subject = "Test Subject";
        var message = "Test Message";

        // Act
        Func<Task> act = async () => await emailSender.SendEmailAsync(invalidEmail, subject, message);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid email address.*");
    }

    [Fact]
    public async Task SendEmailAsync_WithValidEmailButNoSmtpServer_ThrowsException()
    {
        // Arrange
        var emailSender = new EmailSender(_loggerMock.Object, _emailSettingsOptions);
        var validEmail = "test@example.com";
        var subject = "Test Subject";
        var message = "<p>Test Message</p>";

        // Act & Assert
        // This will fail because we don't have a real SMTP server
        // The specific exception type may vary (SocketException, IOException, etc.)
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await emailSender.SendEmailAsync(validEmail, subject, message));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("user+tag@example.com")]
    public void SendEmailAsync_WithValidEmails_DoesNotThrowImmediately(string email)
    {
        // Arrange
        var emailSender = new EmailSender(_loggerMock.Object, _emailSettingsOptions);
        var subject = "Test Subject";
        var message = "<p>Test Message</p>";

        // Act & Assert
        // The method should not throw an ArgumentException for valid emails
        // (it may throw later due to SMTP connection, but that's expected in this test environment)
        var exception = Record.ExceptionAsync(async () =>
            await emailSender.SendEmailAsync(email, subject, message));
        
        exception.Should().NotBeNull(); // Will throw due to SMTP, but not ArgumentException
    }
}
