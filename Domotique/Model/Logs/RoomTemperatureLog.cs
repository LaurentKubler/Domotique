using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model.Logs
{
    class RoomTemperatureLog
    {
        public String Name { get; set; }

        public Double? CurrentTemperature { get; set; }

        public Double? TargetTemperature { get; set; }

        public DateTime LogDate { get; set; }
        
    }
}
