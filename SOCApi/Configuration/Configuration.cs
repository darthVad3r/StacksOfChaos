using Microsoft.Extensions.Options;

namespace SOCApi.Configuration;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int MaxRetries { get; set; }
    public int TimeoutSeconds { get; set; }
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