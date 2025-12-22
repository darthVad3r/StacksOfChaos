using SOCApi.Models;

namespace SOCApi.Services.BookDisplay
{
    public class BookDisplayService : IBookDisplayService
    {
        public Task<string> GetDisplayTitleAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDisplayAuthorAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetShelfLocationAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDisplayStatusAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDisplayISBNAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task DisplayAllBooksAsync(IEnumerable<Models.Book> books)
        {
            throw new NotImplementedException();
        }
    }
}