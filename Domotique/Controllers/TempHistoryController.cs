using Domotique.Database;
using Domotique.Model;
using Messages.WebMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Domotique.Controllers
{
    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public class TempHistoryController : Controller
    {

        IDataRead _dataRead;

        DomotiqueContext _context;


        public TempHistoryController(IDataRead dataRead, DomotiqueContext context)
        {
            _dataRead = dataRead;
            _context = context;
        }


        [HttpGet()]
        public IActionResult Get()
        {
            /*
            using (StreamReader r = new StreamReader("log.json"))
            {
                //  string json = r.ReadToEnd();
                //List<TemperatureLog> items = JsonConvert.DeserializeObject<List<TemperatureLog>>(json);
                
                // items.Where(c => c.RoomId == 4);
                return Ok(_dataRead.ReadRoomTemperaturesGraph());
            }*/
            return Ok(_dataRead.ReadRoomTemperaturesGraph());


        }

        [HttpGet("new")]
        public IActionResult GetNew()
        {
            /*
            using (StreamReader r = new StreamReader("log.json"))
            {
                //  string json = r.ReadToEnd();
                //List<TemperatureLog> items = JsonConvert.DeserializeObject<List<TemperatureLog>>(json);
                
                // items.Where(c => c.RoomId == 4);
                return Ok(_dataRead.ReadRoomTemperaturesGraph());
            }*/
            /*var room = _context.Rooms.Select(r=>new RoomStatus()
            {
                RoomId=r.ID,
                RoomName=r.Name
            });
            var tmp = _context.TemperatureLog.Where(c=>c.ID);
            var test = (from room in _context.Rooms
                        select room);
            */
            var lastTemperatureQuery = _context.TemperatureLog
                        .FromSql(@"
                            SELECT t.* FROM TemperatureLog t
                            WHERE t.LogDate = (SELECT MAX(t2.LogDate) FROM TemperatureLog t2
                            WHERE t2.LogDate = t.LogDate and t2.RoomID = t.RoomID)"
                        );
            var rooms = (from e in _context.Rooms
                         orderby e.Name
                         join t in lastTemperatureQuery on e.ID equals t.RoomID into lastRoomTemperature
                         from t in lastRoomTemperature.DefaultIfEmpty()  // forces left join
                         let lastTest = (from x in lastRoomTemperature select x).FirstOrDefault()
                         select new RoomStatus
                         {
                             RoomId = e.ID,
                             RoomName = e.Name// + t.CurrentTemp.ToString(),
                                              //CurrentTemperature = (float)t.CurrentTemp,
                                              //   LastTemperatureRefresh = t.LogDate
                         }).OrderByDescending(x => x.RoomName);
            /*                        join t in lastTestQuery on e.EngineId equals t.EngineId into lastEngineTest
                                    from t in lastEngineTest.DefaultIfEmpty()  // forces left join
                                    let lastTest = (from x in lastEngineTest select x).FirstOrDefault()
                                    select new EngineVM
                                    {
                                        EngineId = e.EngineId,
                                        Unit = e.Unit,
                                        Make = e.Make,
                                        Model = e.Model,
                                        SerialNumber = e.SerialNumber,
                                        Status = CalculateEngineStatus(e.EngineId,
                                                                       e.PermittedNox,
                                                                       lastTest.Nox)
                                    }).OrderByDescending(x => x.Status);*/
            return Ok(rooms);


        }
    }
}
