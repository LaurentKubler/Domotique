using Domotique.Model.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service.Log
{
    public class LogService : ILogService
    {
        public void LogTemperatureService(String name, Double currentTemperature, Double? targetTemperature, DateTime logDate)
        {
            RoomTemperatureLog tempLog = new RoomTemperatureLog()
            {
                Name = name,
                CurrentTemperature = currentTemperature,
                TargetTemperature = targetTemperature,
                LogDate = logDate
            };
            // TODO add db storage
        }
    }
}
