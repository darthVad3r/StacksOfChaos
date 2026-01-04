using SOCApi.Constants;

namespace SOCApi.Services.BookValidation
{
    public class BookValidationService : IBookValidationService
    {
        public Task<bool> ValidateBookAsync(Models.Book book)
        {
            ArgumentNullException.ThrowIfNull(book, nameof(book));

            ValidateTitle(book.Title);
            ValidateAuthor(book.Author);
            ValidateGenre(book.Genre);
            ValidateYearPublished(book.YearPublished);
            ValidateISBNAsync(book.ISBN).GetAwaiter().GetResult();

            return Task.FromResult(true);
        }

        private static void ValidateTitle(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            
            if (title.Length > ValidBookConstants.TITLE_MAX_LENGTH)
            {
                throw new ArgumentException($"Title cannot exceed {ValidBookConstants.TITLE_MAX_LENGTH} characters.", nameof(title));
            }
        }

        private static void ValidateAuthor(string author)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(author, nameof(author));
            
            if (author.Length > ValidBookConstants.AUTHOR_MAX_LENGTH)
            {
                throw new ArgumentException($"Author name cannot exceed {ValidBookConstants.AUTHOR_MAX_LENGTH} characters.", nameof(author));
            }
        }

        private static void ValidateGenre(string? genre)
        {
            if (!string.IsNullOrWhiteSpace(genre) && genre.Length > ValidBookConstants.GENRE_MAX_LENGTH)
            {
                throw new ArgumentException($"Genre cannot exceed {ValidBookConstants.GENRE_MAX_LENGTH} characters.", nameof(genre));
            }
        }

        private static void ValidateYearPublished(DateTime? yearPublished)
        {
            if (yearPublished.HasValue)
            {
                int year = yearPublished.Value.Year;
                int currentYear = DateTime.Now.Year;
                
                if (year < ValidBookConstants.MIN_PUBLICATION_YEAR || year > currentYear)
                {
                    throw new ArgumentException($"YearPublished must be between {ValidBookConstants.MIN_PUBLICATION_YEAR} and {currentYear}.", nameof(yearPublished));
                }
            }
        }

        public Task<bool> ValidateISBNAsync(string isbn)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(isbn, nameof(isbn));
            
            string cleanedIsbn = CleanIsbn(isbn);
            ValidateIsbnFormat(cleanedIsbn);
            
            return Task.FromResult(true);
        }

        private static string CleanIsbn(string isbn)
        {
            return isbn.Replace("-", "").Replace(" ", "");
        }

        private static void ValidateIsbnFormat(string cleanedIsbn)
        {
            if (cleanedIsbn.Length != 10 && cleanedIsbn.Length != 13)
            {
                throw new ArgumentException("ISBN must be either 10 or 13 digits.", nameof(cleanedIsbn));
            }
            
            if (!cleanedIsbn.All(char.IsDigit))
            {
                throw new ArgumentException("ISBN must contain only digits.", nameof(cleanedIsbn));
            }
        }

        public Task<bool> IsValidYearPublished(int? year)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidAuthorNameAsync(string authorName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(authorName, nameof(authorName));
            
            if (authorName.Length > ValidBookConstants.AUTHOR_MAX_LENGTH)
            {
                throw new ArgumentException($"Author name cannot exceed {ValidBookConstants.AUTHOR_MAX_LENGTH} characters.", nameof(authorName));
            }

            return Task.FromResult(true);
        }
    }
}