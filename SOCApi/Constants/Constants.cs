namespace SOCApi.Constants
{
    public static class DatabaseConstants
    {
        public static class User
        {
            public const int FIRST_NAME_MAX_LENGTH = 50;
            public const int LAST_NAME_MAX_LENGTH = 50;
            public const int PROFILE_PICTURE_URL_MAX_LENGTH = 500;
            public const int BIO_MAX_LENGTH = 1000;
        }

        public static class Role
        {
            public const int DESCRIPTION_MAX_LENGTH = 500;
        }

        public static class RefreshToken
        {
            public const int TOKEN_MAX_LENGTH = 500;
        }

        public const string DEFAULT_TIMESTAMP_SQL = "GETUTCDATE()";
    }

    // Your existing password constants...
    public static class PasswordConstants
    {
        public const int MIN_LENGTH = 8;
        public const int MAX_LENGTH = 128;
        public const int MAX_CONSECUTIVE_CHARACTERS = 3;
        public const int MIN_LENGTH_FOR_REPEATING_CHECK = 4;
        public const string SPECIAL_CHARACTERS = "!@#$%^&*(),.?\":{}|<>";

        public static readonly string[] COMMON_PASSWORDS = {
            "password", "123456", "password123", "admin", "qwerty",
            "letmein", "welcome", "monkey", "1234567890", "password1",
            "abc123", "111111", "123123", "welcome123", "Password1"
        };
    }

    public static class ValidationMessages
    {
        public const string PASSWORD_EMPTY = "Password cannot be empty";
        public const string PASSWORD_TOO_SHORT = "Password must be at least {0} characters long";
        public const string PASSWORD_TOO_LONG = "Password cannot exceed {0} characters";
        public const string PASSWORD_MISSING_UPPERCASE = "Password must contain at least one uppercase letter";
        public const string PASSWORD_MISSING_LOWERCASE = "Password must contain at least one lowercase letter";
        public const string PASSWORD_MISSING_DIGIT = "Password must contain at least one digit";
        public const string PASSWORD_MISSING_SPECIAL = "Password must contain at least one special character (!@#$%^&*(),.?\":{}|<>)";
        public const string PASSWORD_TOO_COMMON = "Password is too common. Please choose a more unique password";
        public const string PASSWORD_TOO_MANY_REPEATING = "Password cannot have more than {0} consecutive identical characters";
    }
}