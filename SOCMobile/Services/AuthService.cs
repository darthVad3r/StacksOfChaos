using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace StacksOfChaos.SOCMobile.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to use for logging.</param>
        public AuthService(ILogger<AuthService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // Initialize HttpClient with a base address for your API
            // Replace "https://your-api-url/" with your actual API URL
            var baseAddress = "https://localhost:5001"; // Replace with configuration value
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    private sealed class LoginResponse
        {
            public string Token { get; set; }
        }
        /// <summary>
        /// Logs in a user with the provided username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the authentication token.</returns>
        /// <exception cref="ArgumentNullException">Thrown when username or password is null or empty.</exception>
        /// <exception cref="HttpRequestException">Thrown when there is an HTTP request error.</exception>
        /// <exception cref="JsonException">Thrown when there is a JSON parsing error.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the login response is invalid.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the token is not found in the response.</exception>
        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                _logger.LogInformation("Logging in user {Username}", username);
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("Username or password cannot be null or empty.");
                }
                var content = new StringContent(
                    JsonSerializer.Serialize(new { username, password }),
                    Encoding.UTF8, "application/json");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Set timeout duration
                var response = await _httpClient.PostAsync("api/auth/login", content, cts.Token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponse>(json);
                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    throw new InvalidOperationException("Invalid login response received.");
                }
                return result.Token;
            }
            catch (HttpRequestException e)
            {
                // Handle HTTP request errors
                _logger.LogError(e, "HTTP error: {Message}", e.Message);
                throw;
            }
            catch (JsonException e)
            {
                // Handle JSON parsing errors
                _logger.LogError(e, "JSON error: {Message}", e.Message);
                throw;
            }
            catch (Exception e)
            {
                // Handle other exceptions
                _logger.LogError(e, "Error: {Message}", e.Message);
                throw;
            }
        }
    }
}