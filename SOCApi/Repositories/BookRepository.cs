using SOCApi.Models;

namespace SOCApi.Repositories
{
    public class BookRepository : IBookRepository
    {
        public async Task<Title?> CreateTitleAsync(Title title)
        {
            // Add the title to the database context
            // Uncomment and modify the following lines according to your data context
            // _context.Titles.Add(title);
            // await _context.SaveChangesAsync();
            return await Task.FromResult<Title?>(title);
        }

        public Task<bool> DeleteTitleByIdAsync(int id)
        {
            // Find the title by ID
            // var title = await _context.Titles.FindAsync(id);
            // if (title == null)
            // {
            //     return false;
            // }

            // Remove the title from the database context
            // _context.Titles.Remove(title);
            // await _context.SaveChangesAsync();

            return Task.FromResult<bool>(true);
        }

        Task<bool?> IBookRepository.DeleteTitleByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
