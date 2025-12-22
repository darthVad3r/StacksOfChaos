using SOCApi.Data;
using SOCApi.Models;
using Microsoft.EntityFrameworkCore;
using SOCApi.Services.Interfaces;

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

        public async Task<Models.Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Models.Book?> CreateBookAsync(string title, string author, string userId, string isbn, DateTime publishedDate)
        {
            var book = new Models.Book(title, author, userId, isbn, publishedDate)
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
            UpdatedAt = DateTime.UtcNow;
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
    }
}