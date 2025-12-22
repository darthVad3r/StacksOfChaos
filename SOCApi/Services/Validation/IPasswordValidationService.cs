using System.Text.RegularExpressions;

namespace SOCApi.Services.Validation
{
    /// <summary>
    /// Service responsible for password validation rules
    /// </summary>
    public interface IPasswordValidationService
    {
        Task<bool> ValidatePasswordAsync(string password);
        Task<bool> IsPasswordStrongEnoughAsync(string password);
        ValidationResult ValidatePasswordStrength(string password);
    }

    /// <summary>
    /// Result of password validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = [];
        
        public static ValidationResult Success() => new() { IsValid = true };
        
        public static ValidationResult Failure(params string[] errors) => new() 
        { 
            IsValid = false, 
            Errors = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>()
        };
    }
}