using Newtonsoft.Json;
using SOCApi.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SOCApi.Services
{
    public class BookSearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BookSearchService> _logger;

        public BookSearchService(IHttpClientFactory httpClientfactory, ILogger<BookSearchService> logger)
        {
            _httpClientFactory = httpClientfactory;
            _logger = logger;
        }
        public async Task<Title?> SearchTitlesAsync(string searchString)
        {
            try
            {
                var title = await FetchTitleDetailsAsync(searchString);
                return title;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for titles with search string {SearchString}", searchString);
                throw;
            }
        }

        private async Task<Title?> FetchTitleDetailsAsync(string nameOfTitle)
        {
            using var client = _httpClientFactory.CreateClient();
            ConfigureHttpClient(client);

            var response = await client.GetAsync($"search.json?q={nameOfTitle}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var searchResult = System.Text.Json.JsonSerializer.Deserialize<Title?>(jsonResponse);
            try
            {
                searchResult = JsonConvert.DeserializeObject<Title>(jsonResponse);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "An error occurred while deserializing the JSON response: {JsonResponse}", jsonResponse);
            }

            if (searchResult == null)
            {
                _logger.LogWarning("No title found for the search string: {NameOfTitle}", nameOfTitle);
            }
            return searchResult;
        }

        private static void ConfigureHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://openlibrary.org/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "SOCApi");
        }
    }
}