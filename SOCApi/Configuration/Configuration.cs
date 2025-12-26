using Microsoft.Extensions.Options;

namespace SOCApi.Configuration;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int MaxRetries { get; set; }
    public int TimeoutSeconds { get; set; }
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Use it in a service
public class SomeService
{
    private readonly ApiSettings _apiSettings;
    
    public SomeService(IOptions<ApiSettings> apiSettings)
    {
        _apiSettings = apiSettings.Value;
    }
}