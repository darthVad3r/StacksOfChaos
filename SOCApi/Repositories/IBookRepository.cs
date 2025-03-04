using SOCApi.Models;

namespace SOCApi.Repositories
{
    public interface IBookRepository
    {
        Task<Title?> CreateTitleAsync(Title title);
        Task<bool?> DeleteTitleAsync(int id);
    }
}
