using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model
{
    public class Room
    {
        public String Name { get; set; }

        public Double? CurrentTemperature { get; set; }

        public Double? TargetTemperature { get; set; }

        public DateTime? LastTemperatureRefreshDate { get; set; }

        Boolean ContainsHeater { get; set; }

        String ProbeName { get; set; }

        String HeaterName { get; set; }

        IList<Schedule> TemperatureSchedule { get; set; }


        public void computeTemperature()
        {

        }
    }
}
