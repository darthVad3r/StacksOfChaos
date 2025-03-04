using SOCApi.Models;

namespace SOCApi.Repositories
{
    public interface IBookRepository
    {
        public Task<Title?> CreateTitleAsync(Title title);
        public Task<bool?> DeleteTitleByIdAsync(int id);
    }
}
