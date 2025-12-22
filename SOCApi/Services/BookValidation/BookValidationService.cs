namespace SOCApi.Services.BookValidation
{
    public class BookValidationService : IBookValidationService
    {
        public Task<bool> ValidateBookAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateISBNAsync(string isbn)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidYearPublished(int? year)
        {
            throw new NotImplementedException();
        }
    }
}