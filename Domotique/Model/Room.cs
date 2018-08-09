using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model
{
    class Room
    {
        String Name { get; set; }

        Double? CurrentTemperature { get; set; }

        Double? TargetTemperature { get; set; }

        DateTime? LastTemperatureRefreshDate { get; set; }

        Boolean ContainsHeater { get; set; }

        String ProbeName { get; set; }

        String HeaterName { get; set; }
    }
}
