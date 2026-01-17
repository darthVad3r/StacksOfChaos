using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SOCApi.Services.Email;
using Xunit;

namespace SOCApi.Tests
{
    public class FileEmailTemplateProviderTests
    {
        /// <summary>
        /// This test creates an actual template file on disk for integration testing.
        /// It validates that the FileEmailTemplateProvider can correctly load and render templates.
        /// </summary>
        [Fact]
        public async Task GetTemplateAsync_WithValidTemplate_LoadsAndRendersCorrectly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var templatesDir = Path.Combine(tempDir, "Templates", "Email");
            Directory.CreateDirectory(templatesDir);

            var templateContent = "Hello {{RecipientName}}, please confirm your email at {{ConfirmationLink}}";
            var templatePath = Path.Combine(templatesDir, "ConfirmEmail.html");
            await File.WriteAllTextAsync(templatePath, templateContent);

            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns(tempDir);

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            var variables = new Dictionary<string, string>
            {
                { "RecipientName", "John Doe" },
                { "ConfirmationLink", "https://example.com/confirm" }
            };

            try
            {
                // Act
                var result = await provider.GetTemplateAsync("ConfirmEmail", variables);

                // Assert
                Assert.NotNull(result);
                Assert.Contains("Hello John Doe", result);
                Assert.Contains("https://example.com/confirm", result);
                Assert.DoesNotContain("{{RecipientName}}", result);
                Assert.DoesNotContain("{{ConfirmationLink}}", result);
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public async Task GetTemplateAsync_WithNullTemplateName_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns("/nonexistent");

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => provider.GetTemplateAsync(null!, new Dictionary<string, string>())
            );
            Assert.Contains("Failed to load email template", exception.Message);
        }

        [Fact]
        public async Task GetTemplateAsync_WithEmptyTemplateName_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns("/nonexistent");

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => provider.GetTemplateAsync("", new Dictionary<string, string>())
            );
            Assert.Contains("Failed to load email template", exception.Message);
        }

        [Fact]
        public async Task GetTemplateAsync_WithNonExistentTemplate_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns("/nonexistent");

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => provider.GetTemplateAsync("NonExistent", new Dictionary<string, string>())
            );
            Assert.Contains("Failed to load email template", exception.Message);
        }

        [Fact]
        public async Task GetTemplateAsync_WithoutVariables_ReturnsTemplateWithPlaceholders()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var templatesDir = Path.Combine(tempDir, "Templates", "Email");
            Directory.CreateDirectory(templatesDir);

            var templateContent = "Hello {{RecipientName}}, please go to {{ConfirmationLink}}";
            var templatePath = Path.Combine(templatesDir, "TestTemplate.html");
            await File.WriteAllTextAsync(templatePath, templateContent);

            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns(tempDir);

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            try
            {
                // Act
                var result = await provider.GetTemplateAsync("TestTemplate", new Dictionary<string, string>());

                // Assert
                Assert.NotNull(result);
                Assert.Contains("{{RecipientName}}", result);
                Assert.Contains("{{ConfirmationLink}}", result);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public async Task GetTemplateAsync_WithPartialVariables_RendersPartially()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var templatesDir = Path.Combine(tempDir, "Templates", "Email");
            Directory.CreateDirectory(templatesDir);

            var templateContent = "Hello {{RecipientName}}, please go to {{ConfirmationLink}}";
            var templatePath = Path.Combine(templatesDir, "TestTemplate.html");
            await File.WriteAllTextAsync(templatePath, templateContent);

            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns(tempDir);

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            var variables = new Dictionary<string, string>
            {
                { "RecipientName", "Jane Smith" }
                // ConfirmationLink is missing
            };

            try
            {
                // Act
                var result = await provider.GetTemplateAsync("TestTemplate", variables);

                // Assert
                Assert.NotNull(result);
                Assert.Contains("Hello Jane Smith", result);
                Assert.Contains("{{ConfirmationLink}}", result);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public async Task GetTemplateAsync_WithSpecialCharactersInVariables_HandlesCorrectly()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var templatesDir = Path.Combine(tempDir, "Templates", "Email");
            Directory.CreateDirectory(templatesDir);

            var templateContent = "Hello {{RecipientName}}, please go to {{ConfirmationLink}}";
            var templatePath = Path.Combine(templatesDir, "TestTemplate.html");
            await File.WriteAllTextAsync(templatePath, templateContent);

            var mockLogger = new Mock<ILogger<FileEmailTemplateProvider>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(x => x.ContentRootPath).Returns(tempDir);

            var provider = new FileEmailTemplateProvider(mockLogger.Object, mockEnvironment.Object);

            var variables = new Dictionary<string, string>
            {
                { "RecipientName", "José García" },
                { "ConfirmationLink", "https://example.com/confirm?token=abc&key=123" }
            };

            try
            {
                // Act
                var result = await provider.GetTemplateAsync("TestTemplate", variables);

                // Assert
                Assert.NotNull(result);
                Assert.Contains("José García", result);
                Assert.Contains("https://example.com/confirm?token=abc&key=123", result);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}


