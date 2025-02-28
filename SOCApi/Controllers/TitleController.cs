using Microsoft.AspNetCore.Mvc;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ILogger<TitleController> _logger;

        public TitleController(ILogger<TitleController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the title information based on the provided search string.
        /// </summary>
        /// <param name="searchString">The search string to look for titles.</param>
        /// <returns>An IActionResult containing the search results or an error message.</returns>
        [HttpGet(Name = "GetTitleInformation")]
        public IActionResult GetTitleInformation([FromQuery] string searchString)
        {
            try
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    _logger.LogError("Search string is empty");
                    return BadRequest("Search string is empty");
                }
                // Implement logic to verify that the search string is valid
                // Implement the logic to search for titles with the search string
                var titles = SearchTitles(searchString);

                if (titles == null || titles.Count == 0)
                {
                    _logger.LogInformation("No titles found for search string {SearchString}", searchString);
                    return NotFound("No titles found");
                }

                return Ok(titles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for titles with search string {SearchString}", searchString);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching for titles");
            }
        }

        private List<string> SearchTitles(string searchString)
        {
            // Implement the logic to search for titles with the search string
            // This is a placeholder implementation
            return new List<string> { "Title1", "Title2", "Title3" };
        }
    }
}