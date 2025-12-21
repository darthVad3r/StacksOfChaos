using FluentAssertions;
using SOCApi.Models;
using SOCApi.Services.BookValidation;

namespace SOCApi.Tests
{
    public class BookValidationServiceTests
    {
        private readonly BookValidationService _validationService;

        public BookValidationServiceTests()
        {
            _validationService = new BookValidationService();
        }

        #region ValidateBookAsync Tests

        [Fact]
        public async Task ValidateBookAsync_WithNullBook_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateBookAsync(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithValidBook_ReturnsTrue()
        {
            // Arrange
            var book = new Book("Test Title", "Test Author", "user123");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateBookAsync_WithEmptyTitle_ReturnsFalse()
        {
            // Arrange
            var book = new Book("", "Test Author", "user123");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithWhitespaceTitle_ReturnsFalse()
        {
            // Arrange
            var book = new Book("   ", "Test Author", "user123");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithEmptyAuthor_ReturnsFalse()
        {
            // Arrange
            var book = new Book("Test Title", "", "user123");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithWhitespaceAuthor_ReturnsFalse()
        {
            // Arrange
            var book = new Book("Test Title", "   ", "user123");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithEmptyUserId_ReturnsFalse()
        {
            // Arrange
            var book = new Book("Test Title", "Test Author", "");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateBookAsync_WithWhitespaceUserId_ReturnsFalse()
        {
            // Arrange
            var book = new Book("Test Title", "Test Author", "   ");

            // Act
            var result = await _validationService.ValidateBookAsync(book);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region ValidateISBNAsync Tests

        [Fact]
        public async Task ValidateISBNAsync_WithNullISBN_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithEmptyISBN_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithWhitespaceISBN_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("   ");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN13_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("9780123456789");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN13WithHyphens_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("978-0-123456-78-9");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN13WithSpaces_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("978 0 123456 78 9");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN10_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("0123456789");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN10WithHyphens_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("0-123-45678-9");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN10WithUppercaseX_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("012345678X");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN10WithLowercaseX_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("012345678x");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithValidISBN10WithXAndHyphens_ReturnsTrue()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("0-123-45678-X");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithInvalidLength_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("12345");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithInvalidCharacters_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("012345678A");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithXInMiddleOfISBN10_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("01234X6789");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithXInISBN13_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("978012345678X");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateISBNAsync_WithTooManyDigits_ReturnsFalse()
        {
            // Act
            var result = await _validationService.ValidateISBNAsync("97801234567890");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsValidYearPublished Tests

        [Fact]
        public async Task IsValidYearPublished_WithNull_ReturnsTrue()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(null);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsValidYearPublished_WithCurrentYear_ReturnsTrue()
        {
            // Arrange
            var currentYear = DateTime.UtcNow.Year;

            // Act
            var result = await _validationService.IsValidYearPublished(currentYear);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsValidYearPublished_WithNextYear_ReturnsTrue()
        {
            // Arrange
            var nextYear = DateTime.UtcNow.Year + 1;

            // Act
            var result = await _validationService.IsValidYearPublished(nextYear);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsValidYearPublished_WithValidHistoricalYear_ReturnsTrue()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(2000);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsValidYearPublished_WithMinimumValidYear_ReturnsTrue()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(1000);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsValidYearPublished_WithYearBelowMinimum_ReturnsFalse()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsValidYearPublished_WithYearTwoYearsInFuture_ReturnsFalse()
        {
            // Arrange
            var twoYearsFromNow = DateTime.UtcNow.Year + 2;

            // Act
            var result = await _validationService.IsValidYearPublished(twoYearsFromNow);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsValidYearPublished_WithNegativeYear_ReturnsFalse()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(-100);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsValidYearPublished_WithZero_ReturnsFalse()
        {
            // Act
            var result = await _validationService.IsValidYearPublished(0);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
