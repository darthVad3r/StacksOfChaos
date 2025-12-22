using SOCApi.Models;

namespace SOCApi.Services.BookStatus
{
    public interface IBookStatusService
    {
        Task MarkAsLentAsync(Models.Book book);
        Task MarkAsAvailableAsync(Models.Book book);
        Task AssignToShelfAsync(Models.Book book, int shelfId);
        Task RemoveFromShelfAsync(Models.Book book);
        bool IsBookAvailable(Models.Book book);
        bool CanBeLent(Models.Book book);
        Task<bool> MarkBookAsLostAsync(int bookId);
        Task<bool> MarkBookAsFoundAsync(int bookId);
        Task<bool> MarkBookAsDamagedAsync(int bookId);
    }
}