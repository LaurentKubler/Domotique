using Domotique.Database;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Domotique.Controllers
{
    [Produces("application/json")]
    [Route("/rest/[controller]")]
    class TempHistoryController : Controller
    {
        [HttpGet()]
        public IActionResult Get()
        {
            using (StreamReader r = new StreamReader("log.json"))
            {
                string json = r.ReadToEnd();
                List<TemperatureLog> items = JsonConvert.DeserializeObject<List<TemperatureLog>>(json);
            }
            return Ok();
        }
    }
}
