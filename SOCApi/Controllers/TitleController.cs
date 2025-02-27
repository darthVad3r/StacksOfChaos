using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SOCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public TitleController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTitle")]
        public IEnumerable<WeatherForecast> Get()
        {
            return null;
        }
    }
}
}
