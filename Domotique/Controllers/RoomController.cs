using Domotique.Database;
using Domotique.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Domotique.Controllers
{


    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public partial class RoomController : Controller
    {
        IDataRead _dataRead;

        DomotiqueContext _context;

        ILogger<StatusController> _logger;

        public RoomController(IDataRead dataRead, DomotiqueContext context, ILogger<StatusController> logger)
        {
            _dataRead = dataRead;
            _context = context;
            _logger = logger;
        }


        [HttpGet("{deviceId}/SetRegulationOn")]
        public IActionResult SetRegulationOn(int roomID)
        {
            var room = _context.Rooms.Where(c => c.ID == roomID).Single();
            room.HeatRegulation = true;
            _context.Update(room);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("{deviceId}/SetRegulationOff")]
        public IActionResult SetRegulationOff(int roomID)
        {
            var room = _context.Rooms.Where(c => c.ID == roomID).Single();
            room.HeatRegulation = false;
            _context.Update(room);
            _context.SaveChanges();

            return Ok();
        }
    }
}
