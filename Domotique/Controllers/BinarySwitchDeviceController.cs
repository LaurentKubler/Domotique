using Domotique.Service;
using Microsoft.AspNetCore.Mvc;

namespace Domotique.Controllers
{
    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public class BinarySwitchDeviceController : Controller
    {
        IDeviceService _deviceService;


        public BinarySwitchDeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }


        [HttpPost("{deviceID}/poweron")]
        public IActionResult PowerOn(long deviceID)
        {
            _deviceService.PowerOn(deviceID);
            return Ok();
        }


        [HttpPost("{deviceID}/poweroff")]
        public IActionResult PowerOff(long deviceID)
        {
            _deviceService.PowerOff(deviceID);
            return Ok();
        }
    }
}
