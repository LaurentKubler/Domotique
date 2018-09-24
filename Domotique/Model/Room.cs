using System;
using System.Collections.Generic;

namespace Domotique.Model
{
    public class Room
    {
        public string Name { get; set; }

        public double? CurrentTemperature { get; set; }

        public double? TargetTemperature { get; set; }

        public DateTime? LastTemperatureRefreshDate { get; set; }

        bool ContainsHeater { get; set; }

        string ProbeName { get; set; }

        string HeaterName { get; set; }

        IList<Schedule> TemperatureSchedule { get; set; }


        public void ComputeTemperature()
        {

        }
    }
}
