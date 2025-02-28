using Microsoft.AspNetCore.Http;
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

        [HttpGet(Name = "GetTitle")]
        public IEnumerable<TitleController> GetTitleInformation(string searchString)
        {
            try
            {
                if(string.IsNullOrEmpty(searchString))
                {
                    _logger.LogError("Search string is empty");
                    return Enumerable.Empty<TitleController>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for titles with search string {SearchString}", searchString);
                return Enumerable.Empty<TitleController>();
            }
        }
    }
}