using FluentAssertions;
using SOCApi.Models;
using SOCApi.Services.BookValidation;

namespace SOCApi.Tests;

public class BookValidationServiceTests
{
    private readonly BookValidationService _service = new();

    #region ValidateBookAsync Tests

    [Fact]
    public async Task ValidateBookAsync_WithValidBook_ReturnsTrue()
    {
        // Arrange
        var book = new Book
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateBookAsync_WithNullBook_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateBookAsync(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithNullTitle_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = null,
            Author = "F. Scott Fitzgerald",
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithEmptyTitle_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = string.Empty,
            Author = "F. Scott Fitzgerald",
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithWhitespaceTitle_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = "   ",
            Author = "F. Scott Fitzgerald",
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithNullAuthor_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = "The Great Gatsby",
            Author = null,
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithEmptyAuthor_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = "The Great Gatsby",
            Author = string.Empty,
            UserId = "user123"
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithNullUserId_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            UserId = null
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateBookAsync_WithEmptyUserId_ReturnsFalse()
    {
        // Arrange
        var book = new Book
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            UserId = string.Empty
        };

        // Act
        var result = await _service.ValidateBookAsync(book);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ValidateISBNAsync Tests

    [Theory]
    [InlineData("9780747532699")] // Valid ISBN-13
    [InlineData("978-0-7475-3269-9")] // Valid ISBN-13 with hyphens
    [InlineData("978 0 7475 3269 9")] // Valid ISBN-13 with spaces
    public async Task ValidateISBNAsync_WithValidISBN13_ReturnsTrue(string isbn)
    {
        // Act
        var result = await _service.ValidateISBNAsync(isbn);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("043942089X")] // Valid ISBN-10 with X
    [InlineData("0439420890")] // Valid ISBN-10
    [InlineData("0-439-42089-X")] // Valid ISBN-10 with hyphens
    [InlineData("0 439 42089 X")] // Valid ISBN-10 with spaces
    public async Task ValidateISBNAsync_WithValidISBN10_ReturnsTrue(string isbn)
    {
        // Act
        var result = await _service.ValidateISBNAsync(isbn);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithNullISBN_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithEmptyISBN_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithWhitespaceISBN_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync("   ");

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("123456789")] // 9 characters
    [InlineData("12345678901")] // 11 characters
    [InlineData("12345678901234")] // 14 characters
    public async Task ValidateISBNAsync_WithInvalidLength_ReturnsFalse(string isbn)
    {
        // Act
        var result = await _service.ValidateISBNAsync(isbn);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithISBN13ContainingNonDigits_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync("978074753269A");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithISBN10WithInvalidLastCharacter_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync("043942089A");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateISBNAsync_WithISBN10WithNonDigitInFirstNine_ReturnsFalse()
    {
        // Act
        var result = await _service.ValidateISBNAsync("04394208A0");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsValidYearPublished Tests

    [Fact]
    public async Task IsValidYearPublished_WithNullYear_ReturnsTrue()
    {
        // Act
        var result = await _service.IsValidYearPublished(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidYearPublished_WithCurrentYear_ReturnsTrue()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        var yearDateTime = new DateTime(currentYear, 1, 1);

        // Act
        var result = await _service.IsValidYearPublished(yearDateTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidYearPublished_WithPastYear_ReturnsTrue()
    {
        // Arrange
        var pastYear = DateTime.UtcNow.AddYears(-10).Year;
        var yearDateTime = new DateTime(pastYear, 1, 1);

        // Act
        var result = await _service.IsValidYearPublished(yearDateTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidYearPublished_WithFutureYear_ReturnsFalse()
    {
        // Arrange
        var futureYear = DateTime.UtcNow.AddYears(1).Year;
        var yearDateTime = new DateTime(futureYear, 1, 1);

        // Act
        var result = await _service.IsValidYearPublished(yearDateTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidYearPublished_WithYearBefore1000_ReturnsFalse()
    {
        // Arrange
        var yearDateTime = new DateTime(999, 1, 1);

        // Act
        var result = await _service.IsValidYearPublished(yearDateTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidYearPublished_WithYear1000_ReturnsTrue()
    {
        // Arrange
        var yearDateTime = new DateTime(1000, 1, 1);

        // Act
        var result = await _service.IsValidYearPublished(yearDateTime);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
