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
            
            // Check if it's 10 or 13 characters
            var isValid = cleanISBN.Length == 10 || cleanISBN.Length == 13;
            
            if (isValid)
            {
                if (cleanISBN.Length == 13)
                {
                    // ISBN-13 must be all digits
                    isValid = cleanISBN.All(char.IsDigit);
                }
                else // cleanISBN.Length == 10
                {
                    // ISBN-10: first 9 characters must be digits,
                    // last character can be a digit or 'X' (check digit)
                    var firstNineAreDigits = cleanISBN.Take(9).All(char.IsDigit);
                    var lastChar = cleanISBN[9];
                    var lastIsValid = char.IsDigit(lastChar) || lastChar == 'X' || lastChar == 'x';
                    isValid = firstNineAreDigits && lastIsValid;
                }
            }
            
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