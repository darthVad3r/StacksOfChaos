namespace SOCApi.Services.BookStatus
{
    public class BookStatusService : IBookStatusService
    {
        public Task MarkAsLentAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsAvailableAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task AssignToShelfAsync(Models.Book book, int shelfId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromShelfAsync(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public bool IsBookAvailable(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public bool CanBeLent(Models.Book book)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkBookAsLostAsync(int bookId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkBookAsFoundAsync(int bookId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkBookAsDamagedAsync(int bookId)
        {
            throw new NotImplementedException();
        }
    }
}