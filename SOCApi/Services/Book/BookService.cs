using SOCApi.Data;
using SOCApi.Models;
using Microsoft.EntityFrameworkCore;
using SOCApi.Services.Common;
using SOCApi.Services.BookValidation;
using SOCApi.Models.Enums;

namespace SOCApi.Services.Book
{
    public class BookService : IBookService
    {
        private readonly SocApiDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBookValidationService _bookValidationService;

        public BookService(SocApiDbContext context, IDateTimeProvider dateTimeProvider, IBookValidationService bookValidationService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _bookValidationService = bookValidationService;
        }

        public async Task<List<Models.Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Models.Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Models.Book?> CreateBookAsync(Models.Book mybook)
        {
            try
            {
                await _bookValidationService.ValidateBookAsync(mybook);
            }
            catch (Exception ex)
            {
                // Handle validation exception (log it, rethrow it, etc.)
                throw new ArgumentException("Book validation failed: " + ex.Message);
            }
            //var book = new Models.Book(title, author, userId, isbn, publishedDate);
            var book = new Models.Book
            {
                Title = mybook.Title,
                Author = mybook.Author,
                UserId = mybook.UserId,
                ISBN = mybook.ISBN,
                YearPublished = mybook.YearPublished,
                CreatedAt = _dateTimeProvider.UtcNow,
                UpdatedAt = _dateTimeProvider.UtcNow,
                IsActive = true,
                Status = Models.Enums.BookStatus.Available,
                CoverImageBlob = mybook.CoverImageBlob,
                Condition = BookCondition.Good
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Models.Book?> UpdateBookAsync(int id, Models.Book book)
        {
            if (id != book.Id) return null;

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public void UpdateTimestamp()
        {
            var UpdatedAt = DateTime.UtcNow;
        }

        public bool IsValidISBNAsync(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return false;

            // Basic ISBN validation (you can make this more sophisticated)
            var cleanISBN = isbn.Replace("-", "").Replace(" ", "");
            return cleanISBN.Length == 10 || cleanISBN.Length == 13;

        }

        public Task<Models.Book?> CreateBookAsync(string title, string author, string isbn, DateTime publishedDate)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Book?> UpdateBookAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Book?> GetBookByISBNAsync(string isbn)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Models.Book>> GetBooksByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Models.Book>> IBookService.GetAllBooksAsync()
        {
            throw new NotImplementedException();
        }
    }
}