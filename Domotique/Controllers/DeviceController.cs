using Microsoft.AspNetCore.Mvc;

namespace Domotique.Controllers
{
    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public class DeviceController : Controller
    {
        public DeviceController()
        {
        }


        [HttpPost()]
        public IActionResult Post(long DeviceID, long Value)
        {
            return Ok();
        }
    }
}
