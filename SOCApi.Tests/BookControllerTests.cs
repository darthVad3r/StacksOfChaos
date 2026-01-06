using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SOCApi.Controllers;
using SOCApi.Models;
using SOCApi.Services.Book;
using Xunit;

namespace SOCApi.Tests
{
    /// <summary>
    /// Unit tests for BookController.
    /// Tests CRUD operations, authorization, and error handling.
    /// </summary>
    public class BookControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IBookService> _mockBookService;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            _mockUserManager = CreateMockUserManager();
            _mockBookService = new Mock<IBookService>();
            _controller = new BookController(_mockUserManager.Object, _mockBookService.Object);
        }

        #region Get All Books Tests

        [Fact]
        public async Task GetAllBooks_WhenBooksExist_ReturnsOkWithBooks()
        {
            // Arrange
            var books = new[] { CreateSampleBook(1), CreateSampleBook(2) };
            _mockBookService.Setup(x => x.GetAllBooksAsync())
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count());
        }

        [Fact]
        public async Task GetAllBooks_WhenNoBooksExist_ReturnsEmptyList()
        {
            // Arrange
            _mockBookService.Setup(x => x.GetAllBooksAsync())
                .ReturnsAsync(Enumerable.Empty<Book>());

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Empty(returnedBooks);
        }

        [Fact]
        public async Task GetAllBooks_CallsBookService()
        {
            // Arrange
            _mockBookService.Setup(x => x.GetAllBooksAsync())
                .ReturnsAsync(Enumerable.Empty<Book>());

            // Act
            await _controller.GetAllBooks();

            // Assert
            _mockBookService.Verify(x => x.GetAllBooksAsync(), Times.Once);
        }

        #endregion

        #region Get Book By Id Tests

        [Fact]
        public async Task GetBookById_WithValidId_ReturnsOkWithBook()
        {
            // Arrange
            var bookId = 1;
            var book = CreateSampleBook(bookId);
            _mockBookService.Setup(x => x.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(bookId, returnedBook.Id);
        }

        [Fact]
        public async Task GetBookById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var bookId = 999;
            _mockBookService.Setup(x => x.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetBookById_WithValidIds_CallsServiceWithCorrectId(int bookId)
        {
            // Arrange
            _mockBookService.Setup(x => x.GetBookByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Book?)null);

            // Act
            await _controller.GetBookById(bookId);

            // Assert
            _mockBookService.Verify(x => x.GetBookByIdAsync(bookId), Times.Once);
        }

        #endregion

        #region Create Book Tests

        [Fact]
        public async Task CreateBook_WithValidBook_ReturnsCreatedAtAction()
        {
            // Arrange
            var bookToCreate = CreateSampleBook();
            var createdBook = CreateSampleBook(1);
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(createdBook);

            // Act
            var result = await _controller.CreateBook(bookToCreate);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(BookController.GetBookById), createdResult.ActionName);
            Assert.Equal(createdBook.Id, ((Book)createdResult.Value!).Id);
        }

        [Fact]
        public async Task CreateBook_WithValidBook_CallsBookService()
        {
            // Arrange
            var bookToCreate = CreateSampleBook();
            var createdBook = CreateSampleBook(1);
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(createdBook);

            // Act
            await _controller.CreateBook(bookToCreate);

            // Assert
            _mockBookService.Verify(x => x.CreateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task CreateBook_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var book = new Book();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.CreateBook(book);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateBook_WithValidBook_ReturnsCreatedBook()
        {
            // Arrange
            var bookToCreate = CreateSampleBook();
            var expectedBook = CreateSampleBook(1, "Test Book");
            _mockBookService.Setup(x => x.CreateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _controller.CreateBook(bookToCreate);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedBook = Assert.IsType<Book>(createdResult.Value);
            Assert.Equal(expectedBook.Title, returnedBook.Title);
        }

        #endregion

        #region Update Book Tests

        [Fact]
        public async Task UpdateBook_WithValidBook_ReturnsOk()
        {
            // Arrange
            var bookId = 1;
            var bookToUpdate = CreateSampleBook(bookId);
            _mockBookService.Setup(x => x.UpdateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(bookToUpdate);

            // Act
            var result = await _controller.UpdateBook(bookId, bookToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateBook_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var bookId = 999;
            var bookToUpdate = CreateSampleBook(bookId);
            _mockBookService.Setup(x => x.UpdateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _controller.UpdateBook(bookId, bookToUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateBook_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var bookId = 1;
            var book = new Book();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.UpdateBook(bookId, book);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateBook_SetsBookIdFromRoute()
        {
            // Arrange
            var bookId = 5;
            var book = new Book { Id = 0, Title = "Test" };
            var updatedBook = CreateSampleBook(bookId);
            _mockBookService.Setup(x => x.UpdateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(updatedBook);

            // Act
            await _controller.UpdateBook(bookId, book);

            // Assert
            _mockBookService.Verify(x => x.UpdateBookAsync(
                It.Is<Book>(b => b.Id == bookId)), Times.Once);
        }

        [Fact]
        public async Task UpdateBook_WithValidData_ReturnsUpdatedBook()
        {
            // Arrange
            var bookId = 1;
            var bookToUpdate = CreateSampleBook(bookId, "Updated Title");
            _mockBookService.Setup(x => x.UpdateBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(bookToUpdate);

            // Act
            var result = await _controller.UpdateBook(bookId, bookToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal("Updated Title", returnedBook.Title);
        }

        #endregion

        #region Delete Book Tests

        [Fact]
        public async Task DeleteBook_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var bookId = 1;
            _mockBookService.Setup(x => x.DeleteBookAsync(bookId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var bookId = 999;
            _mockBookService.Setup(x => x.DeleteBookAsync(bookId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBook(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBook_WithValidId_CallsBookService()
        {
            // Arrange
            var bookId = 1;
            _mockBookService.Setup(x => x.DeleteBookAsync(bookId))
                .ReturnsAsync(true);

            // Act
            await _controller.DeleteBook(bookId);

            // Assert
            _mockBookService.Verify(x => x.DeleteBookAsync(bookId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task DeleteBook_WithVariousIds_CallsServiceCorrectly(int bookId)
        {
            // Arrange
            _mockBookService.Setup(x => x.DeleteBookAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            await _controller.DeleteBook(bookId);

            // Assert
            _mockBookService.Verify(x => x.DeleteBookAsync(bookId), Times.Once);
        }

        #endregion

        #region Controller Initialization Tests

        [Fact]
        public void BookController_WithValidDependencies_Initializes()
        {
            // Act
            var controller = new BookController(_mockUserManager.Object, _mockBookService.Object);

            // Assert
            Assert.NotNull(controller);
        }

        [Fact]
        public void BookController_WithNullUserManager_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookController(null!, _mockBookService.Object));
        }

        [Fact]
        public void BookController_WithNullBookService_Throws()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookController(_mockUserManager.Object, null!));
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
        /// Creates a sample book for testing.
        /// </summary>
        private static Book CreateSampleBook(int id = 0, string title = "Sample Book")
        {
            return new Book
            {
                Id = id,
                Title = title,
                Author = "Test Author",
                UserId = "test-user-id",
                ISBN = "123-456-789",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        #endregion
    }
}
