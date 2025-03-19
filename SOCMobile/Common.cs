namespace SOCMobile
{
    public static class Common
    {
        public const string AppName = "SOC Mobile";
        public const string AppVersion = "1.0.0";
        public const string AppDescription = "A mobile application for SOC management.";
        
        // Add any other common constants or methods here
        public static string GetAppInfo()
        {
            return $"{AppName} - {AppVersion}: {AppDescription}";
        }
        public static string GetApiBaseUrl()
        {
            // Replace with your actual API base URL
            return "https://localhost:5001"; // This should ideally come from configuration
        }
    }
}