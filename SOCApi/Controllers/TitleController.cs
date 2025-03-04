﻿using Microsoft.AspNetCore.Mvc;
using SOCApi.Services;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ILogger<TitleController> _logger;
        private readonly BookSearchService _bookSearchService;

        public TitleController(ILogger<TitleController> logger, BookSearchService bookSearchService)
        {
            _bookSearchService = bookSearchService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the title information based on the provided search string.
        /// </summary>
        /// <param name="searchString">The search string to look for titles.</param>
        /// <returns>An IActionResult containing the search results or an error message.</returns>
        [HttpGet(Name = "GetTitleInformation")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTitleInformation([FromQuery] string searchString)
        {
            try
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    _logger.LogError("Search string is empty");
                    return BadRequest("Search string cannot empty");
                }
                // Implement logic to verify that the search string is valid
                // Implement the logic to search for titles with the search string
                var titles = _bookSearchService.SearchTitlesAsync(searchString).Result;

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
    }
}