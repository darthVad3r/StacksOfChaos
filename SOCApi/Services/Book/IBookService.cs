using System.Runtime.CompilerServices;
using SOCApi.Models;

namespace SOCApi.Services.Book
{
    public interface IBookService
    {
        Task<Models.Book?> CreateBookAsync(string title, string author, string isbn, DateTime publishedDate);
        Task<Models.Book?> UpdateBookAsync(Models.Book book);
        Task<bool> DeleteBookAsync(int bookId);
        Task<Models.Book?> GetBookByIdAsync(int bookId);
        Task<IEnumerable<Models.Book>> GetAllBooksAsync();

        // This method should implement the book lookup api call to the 
        // external book look up service like openlibrary.org
        Task<Models.Book?> GetBookByISBNAsync(string isbn);
        Task<IEnumerable<Models.Book>> GetBooksByUserAsync(string userId);
    }
}