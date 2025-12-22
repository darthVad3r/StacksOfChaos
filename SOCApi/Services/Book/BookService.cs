using SOCApi.Data;
using SOCApi.Models;
using Microsoft.EntityFrameworkCore;
using SOCApi.Services.Common;
using SOCApi.Services.BookValidation;

namespace SOCApi.Services.Book
{
    public class BookService : IBookService
    {
        private readonly SocApiDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IBookValidationService _bookValidationService;

        public BookService(SOCApi.Data.SocApiDbContext context, IDateTimeProvider dateTimeProvider, IBookValidationService bookValidationService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _bookValidationService = bookValidationService;
        }

        public async Task<List<Models.Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Models.Book?> GetBookByIdAsync(int bookId)
        {
            return await _context.Books.FindAsync(bookId);
        }

        public async Task<Models.Book?> CreateBookAsync(Models.Book book)
        {
            if (!await _bookValidationService.ValidateBookAsync(book))
            {
                // Invalid book data
                // Log the validation failure as needed
                return null;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Models.Book?> UpdateBookAsync(int id, Models.Book book)
        {
            if (id != book.Id) return null;

            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return book;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExistsAsync(book.Id))
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<Models.Book?> UpdateBookAsync(Models.Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return book;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExistsAsync(book.Id))
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Models.Book?> GetBookByISBNAsync(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task<IEnumerable<Models.Book>> GetBooksByUserAsync(string userId)
        {
            return await _context.Books.Where(b => b.UserId == userId).ToListAsync();
        }

        private async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }

        public static bool IsValidISBN(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return false;

            // Basic ISBN validation
            var cleanISBN = isbn.Replace("-", "").Replace(" ", "");
            return cleanISBN.Length == 10 || cleanISBN.Length == 13;
        }
    }
}