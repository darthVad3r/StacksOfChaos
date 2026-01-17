using SOCApi.Data;
using SOCApi.Models;
using Microsoft.EntityFrameworkCore;
using SOCApi.Services.Common;
using SOCApi.Services.BookValidation;
using SOCApi.Models.Enums;

namespace SOCApi.Services.Book
{
    /// <summary>
    /// Service for managing book operations including CRUD operations,
    /// user book management, and ISBN lookups.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly SocApiDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBookValidationService _bookValidationService;
        private readonly ILogger<BookService> _logger;

        public BookService(
            SocApiDbContext context,
            IDateTimeProvider dateTimeProvider,
            IBookValidationService bookValidationService,
            ILogger<BookService> logger)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _bookValidationService = bookValidationService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all active books from the database.
        /// </summary>
        public async Task<IEnumerable<Models.Book>> GetAllBooksAsync()
        {
            try
            {
                return await _context.Books
                    .Where(b => b.IsActive)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific book by its ID.
        /// </summary>
        public async Task<Models.Book?> GetBookByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid book ID requested: {BookId}", id);
                    return null;
                }

                return await _context.Books
                    .Include(b => b.User)
                    .Include(b => b.Shelf)
                    .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID: {BookId}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new book with validation.
        /// </summary>
        public async Task<Models.Book?> CreateBookAsync(Models.Book book)
        {
            try
            {
                if (book == null)
                {
                    throw new ArgumentNullException(nameof(book));
                }

                // Validate the book
                await _bookValidationService.ValidateBookAsync(book);

                // Create the book entity
                var newBook = new Models.Book
                {
                    Title = book.Title,
                    Author = book.Author,
                    UserId = book.UserId,
                    ISBN = book.ISBN,
                    YearPublished = book.YearPublished,
                    Genre = book.Genre,
                    Description = book.Description,
                    CoverImageUrl = book.CoverImageUrl,
                    Publisher = book.Publisher,
                    PageCount = book.PageCount,
                    Language = book.Language ?? "English",
                    Price = book.Price,
                    PurchaseDate = book.PurchaseDate,
                    CreatedAt = _dateTimeProvider.UtcNow,
                    UpdatedAt = _dateTimeProvider.UtcNow,
                    IsActive = true,
                    Status = SOCApi.Models.Enums.BookStatus.Available,
                    Condition = book.Condition,
                    CoverImageBlob = book.CoverImageBlob,
                    ShelfId = book.ShelfId
                };

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book created successfully: {BookId}", newBook.Id);
                return newBook;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Book validation failed");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        public async Task<Models.Book?> UpdateBookAsync(Models.Book book)
        {
            try
            {
                if (book == null)
                {
                    throw new ArgumentNullException(nameof(book));
                }

                if (book.Id <= 0)
                {
                    throw new ArgumentException("Invalid book ID", nameof(book.Id));
                }

                // Check if book exists
                var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
                if (existingBook == null)
                {
                    _logger.LogWarning("Book not found for update: {BookId}", book.Id);
                    return null;
                }

                // Validate the book
                await _bookValidationService.ValidateBookAsync(book);

                // Update properties
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.YearPublished = book.YearPublished;
                existingBook.Genre = book.Genre;
                existingBook.Description = book.Description;
                existingBook.CoverImageUrl = book.CoverImageUrl;
                existingBook.Publisher = book.Publisher;
                existingBook.PageCount = book.PageCount;
                existingBook.Language = book.Language ?? "English";
                existingBook.Price = book.Price;
                existingBook.PurchaseDate = book.PurchaseDate;
                existingBook.Condition = book.Condition;
                existingBook.Status = book.Status;
                existingBook.CoverImageBlob = book.CoverImageBlob;
                existingBook.ShelfId = book.ShelfId;
                existingBook.UpdatedAt = _dateTimeProvider.UtcNow;

                _context.Entry(existingBook).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book updated successfully: {BookId}", existingBook.Id);
                return existingBook;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Book validation failed during update");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book: {BookId}", book?.Id);
                throw;
            }
        }

        /// <summary>
        /// Deletes a book (soft delete via IsActive flag).
        /// </summary>
        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid book ID for deletion: {BookId}", id);
                    return false;
                }

                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
                if (book == null)
                {
                    _logger.LogWarning("Book not found for deletion: {BookId}", id);
                    return false;
                }

                // Soft delete
                book.IsActive = false;
                book.DeletedAt = _dateTimeProvider.UtcNow;
                book.UpdatedAt = _dateTimeProvider.UtcNow;

                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Book deleted successfully: {BookId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book: {BookId}", id);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a book by ISBN (if found in database).
        /// Note: For external lookups to services like OpenLibrary, implement a separate service.
        /// </summary>
        public async Task<Models.Book?> GetBookByISBNAsync(string isbn)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    _logger.LogWarning("Empty ISBN provided for lookup");
                    return null;
                }

                // Clean the ISBN
                var cleanIsbn = isbn.Replace("-", "").Replace(" ", "").Trim();

                return await _context.Books
                    .Include(b => b.User)
                    .Include(b => b.Shelf)
                    .FirstOrDefaultAsync(b => b.ISBN != null && b.ISBN.Replace("-", "").Replace(" ", "") == cleanIsbn && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book by ISBN: {ISBN}", isbn);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all books owned by a specific user.
        /// </summary>
        public async Task<IEnumerable<Models.Book>> GetBooksByUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("Empty user ID provided for book lookup");
                    return Enumerable.Empty<Models.Book>();
                }

                return await _context.Books
                    .Where(b => b.UserId == userId && b.IsActive)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books for user: {UserId}", userId);
                throw;
            }
        }
    }
}