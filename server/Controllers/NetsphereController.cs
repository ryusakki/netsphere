using Microsoft.AspNetCore.Mvc;

namespace Netsphere.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Netsphere : ControllerBase
    {
        [HttpPost("ping")]
        public ActionResult<string> Ping() => Ok("Pong");
    }
}
