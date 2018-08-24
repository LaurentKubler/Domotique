using Domotique.Model;
using Microsoft.AspNetCore.Mvc;

namespace Domotique.Controllers
{
    

    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public class StatusController : Controller
    {
        IDataRead _dataRead;


        public StatusController(IDataRead dataRead)
        {
            _dataRead = dataRead;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(_dataRead.ReadRoomTemperatures());
        }
    }
}
