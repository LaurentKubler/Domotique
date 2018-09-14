using Domotique.Database;
using Domotique.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Domotique.Controllers
{
    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public class TempHistoryController : Controller
    {

        IDataRead _dataRead;


        public TempHistoryController(IDataRead dataRead)
        {
            _dataRead = dataRead;
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
    }
}
