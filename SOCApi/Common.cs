// This file is part of the SOCApi project.
namespace SOCApi
{
    
    public static class Common
    {
        public static string GetConnectionString()
        {
            return "Server=localhost;Database=SOCApi;User Id=sa;Password=your_password;";
        }
        public static string GetAllowedOrigin()
        {
            return "https://localhost:52454";
        }
        public static class Endpoints
        {
            public const string BASE_URL = "https://localhost:52454";
            public const string GOOGLE_LOGIN = "/api/auth/google-login";
            public const string GOOGLE_CALLBACK_URI = "/api/auth/google-callback";
            public const string TEST_CONNECTION = "/api/auth/test-connection";
            public const string CLIENT_REDIRECT_URI = BASE_URL + "/api/auth/google-callback";
        }

        public static class StoredProcedures
        {
            public const string VALIDATE_EMAIL_UNIQUE = "usp_ValidateEmailUnique";
            public const string VALIDATE_USERNAME_UNIQUE = "usp_ValidateUsernameUnique";
            public const string GET_USERS_BY_EMAIL = "usp_GetUsersByEmail";
            public const string CREATE_USER = "usp_CreateUser";
            public const string VERIFY_IF_USER_EXISTS = "usp_VerifyIfUserExists";
        }
    }
}