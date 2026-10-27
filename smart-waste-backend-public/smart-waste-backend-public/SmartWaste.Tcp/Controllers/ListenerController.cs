using Microsoft.AspNetCore.Mvc;

namespace SmartWaste.Tcp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListenerController : ControllerBase
    {
        private readonly ILogger<ListenerController> _logger;

        public ListenerController(ILogger<ListenerController> logger)
        {
            _logger = logger;
        }
    }
}

