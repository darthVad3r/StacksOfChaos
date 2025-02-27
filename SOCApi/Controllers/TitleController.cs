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
        public IEnumerable<TitleController> Get()
        {
            return null;
        }
    }
}