using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SOCApi.Constants;

namespace SOCApi.Services.Validation
{
    /// <summary>
    /// Service for validating password strength and rules
    /// </summary>
    public class PasswordValidationService : IPasswordValidationService
    {
        private readonly ILogger<PasswordValidationService> _logger;

        private const int MIN_LENGTH = PasswordConstants.MIN_LENGTH;
        private const int MAX_LENGTH = PasswordConstants.MAX_LENGTH;
        private const string SPECIAL_CHARACTERS = PasswordConstants.SPECIAL_CHARACTERS;

        public PasswordValidationService(ILogger<PasswordValidationService> logger)
        {
            _logger = logger;
        }

        public Task<bool> ValidatePasswordAsync(string password)
        {
            var result = ValidatePasswordStrength(password);
            return Task.FromResult(result.IsValid);
        }

        public Task<bool> IsPasswordStrongEnoughAsync(string password)
        {
            var result = ValidatePasswordStrength(password);
            return Task.FromResult(result.IsValid);
        }

        public ValidationResult ValidatePasswordStrength(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                return ValidationResult.Failure(ValidationMessages.PASSWORD_EMPTY);
            }

            // Length validation
            if (password.Length < MIN_LENGTH)
            {
                errors.Add(string.Format(ValidationMessages.PASSWORD_TOO_SHORT, MIN_LENGTH));
            }

            if (password.Length > MAX_LENGTH)
            {
                errors.Add(string.Format(ValidationMessages.PASSWORD_TOO_LONG, MAX_LENGTH));
            }

            // Character type requirements
            if (!HasUppercase(password))
            {
                errors.Add(ValidationMessages.PASSWORD_MISSING_UPPERCASE);
            }

            if (!HasLowercase(password))
            {
                errors.Add(ValidationMessages.PASSWORD_MISSING_LOWERCASE);
            }

            if (!HasDigit(password))
            {
                errors.Add(ValidationMessages.PASSWORD_MISSING_DIGIT);
            }

            if (!HasSpecialCharacter(password))
            {
                errors.Add(ValidationMessages.PASSWORD_MISSING_SPECIAL);
            }

            // Common password checks
            if (IsCommonPassword(password))
            {
                errors.Add(ValidationMessages.PASSWORD_TOO_COMMON);
            }

            if (HasRepeatingCharacters(password))
            {
                errors.Add(string.Format(ValidationMessages.PASSWORD_TOO_MANY_REPEATING, PasswordConstants.MAX_CONSECUTIVE_CHARACTERS));
            }

            return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors.ToArray());
        }
        
        private static bool HasUppercase(string password) =>
            !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);

        private static bool HasLowercase(string password) =>
            !string.IsNullOrEmpty(password) && password.Any(char.IsLower);

        private static bool HasDigit(string password) =>
            !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);

        private static bool HasSpecialCharacter(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            try
            {
                return Regex.IsMatch(password, @"{SpecialCharacters}" + PasswordConstants.SPECIAL_CHARACTERS.Replace("]", @"\]").Replace("[", @"\["), RegexOptions.ECMAScript);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsCommonPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            var commonPasswords = new HashSet<string>(PasswordConstants.COMMON_PASSWORDS, StringComparer.OrdinalIgnoreCase);
            return commonPasswords.Contains(password);
        }

        private static bool HasRepeatingCharacters(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < PasswordConstants.MIN_LENGTH_FOR_REPEATING_CHECK)
                return false;

            for (int i = 0; i < password.Length - PasswordConstants.MAX_CONSECUTIVE_CHARACTERS; i++)
            {
                if (password[i] == password[i + 1] &&
                    password[i] == password[i + 2] &&
                    password[i] == password[i + 3])
                {
                    return true;
                }
            }
            return false;
        }
    }
}