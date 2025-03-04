using Newtonsoft.Json;
using SOCApi.Models;
using System.Net.Http.Headers;

namespace SOCApi.Services
{
    public class BookSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookSearchService> _logger;

        public BookSearchService(HttpClient httpClient, ILogger<BookSearchService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<List<Title>> SearchTitlesAsync(string searchString)
        {
            // Implement the logic to search for titles using the search string
            // This is a placeholder implementation
            var titles = new List<Title>
            {
                new Title { Id = 1, Name = "Title1" },
                new Title { Id = 2, Name = "Title2" },
                new Title { Id = 3, Name = "Title3" }
            };
            return await Task.FromResult(titles);
        }
    }
}