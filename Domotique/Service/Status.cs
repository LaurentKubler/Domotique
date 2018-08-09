using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    public class Status
    {
        readonly TemperatureReadingService TempReadingService;
        public Status(TemperatureReadingService tempReadingService)
        {
            TempReadingService = tempReadingService;
            TempReadingService.Start();
        }
    }
}
