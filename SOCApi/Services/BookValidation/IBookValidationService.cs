using System.Runtime.CompilerServices;

namespace SOCApi.Services.BookValidation
{
    public interface IBookValidationService
    {
        Task<bool> ValidateBookAsync(Models.Book book);
        Task<bool> ValidateISBNAsync(string isbn);
        Task<bool> IsValidYearPublished(int? year);
    }
}