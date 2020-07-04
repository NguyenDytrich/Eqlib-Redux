using Microsoft.AspNetCore.Mvc;

namespace EqlibApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public string Ping()
        {
            return "pong";
        }
    }
}