using System;
using System.Linq;
using System.Threading.Tasks;
using SOCApi.Models;

namespace SOCApi.Services.BookValidation
{
    public class BookValidationService : IBookValidationService
    {
        public Task<bool> ValidateBookAsync(Models.Book book)
        {
            if (book == null) return Task.FromResult(false);
            
            // Basic book validation
            var isValid = !string.IsNullOrWhiteSpace(book.Title) && 
                         !string.IsNullOrWhiteSpace(book.Author) &&
                         !string.IsNullOrWhiteSpace(book.UserId);
            
            return Task.FromResult(isValid);
        }

        public Task<bool> ValidateISBNAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return Task.FromResult(false);
            
            // Basic ISBN validation - remove hyphens and spaces
            var cleanISBN = isbn.Replace("-", "").Replace(" ", "");
            
            // Check if it's 10 or 13 digits
            var isValid = cleanISBN.Length == 10 || cleanISBN.Length == 13;
            
            // Additional validation: ensure all characters are digits
            isValid = isValid && cleanISBN.All(char.IsDigit);
            
            return Task.FromResult(isValid);
        }

        public Task<bool> IsValidYearPublished(int? year)
        {
            if (!year.HasValue) return Task.FromResult(true); // Year is optional
            
            var currentYear = DateTime.UtcNow.Year;
            var isValid = year.Value >= 1000 && year.Value <= currentYear + 1; // Allow next year for pre-releases
            
            return Task.FromResult(isValid);
        }
    }
}