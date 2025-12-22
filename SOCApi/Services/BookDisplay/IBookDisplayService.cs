namespace SOCApi.Services.BookDisplay
{
    public interface IBookDisplayService
    {
        Task<string> GetDisplayTitleAsync(Models.Book book);
        Task<string> GetDisplayAuthorAsync(Models.Book book);
        Task<string> GetShelfLocationAsync(Models.Book book);
        Task<string> GetDisplayStatusAsync(Models.Book book);
        Task<string> GetDisplayISBNAsync(Models.Book book);
        Task DisplayAllBooksAsync(IEnumerable<Models.Book> books);
    }
}