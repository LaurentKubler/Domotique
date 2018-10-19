using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domotique.Database
{
    [Table("Room")]
    public class Room
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        [Column("Captor")]
        [ForeignKey("Device")]
        public int? CaptorID { get; set; }

        public Device Captor { get; set; }

        [Column("Heater")]
        [ForeignKey("Device")]
        public int? HeaterID { get; set; }

        public Device Heater { get; set; }


        public float? MinTemperature { get; set; }


        public float? ComfortTemperature { get; set; }

        public bool HeatRegulation { get; set; }

        public IList<TemperatureSchedule> TemperatureSchedules { get; set; }

        [NotMapped]
        public double CurrentTemperature { get; set; }

        [NotMapped]
        public double? TargetTemperature { get; set; } = null;

        [NotMapped]
        public DateTime LastTemperatureRefreshDate { get; set; }


        public void ComputeCurrentTemperature()
        {
            ComputeTemperature(DateTime.Now);
        }


        public void ComputeTemperature(DateTime time)
        {
            if (!HeatRegulation)
            {
                TargetTemperature = null;
                return;
            }

            if (TemperatureSchedules.Count != 0)
            {
                var schedules = TemperatureSchedules.OrderBy(schedule => schedule.Priority);

                foreach (var schedule in schedules)
                {
                    if (schedule.Schedule.IsActiveOn(time))
                    {
                        TargetTemperature = schedule.TargetTemperature;
                        return;
                    }
                }
            }

            TargetTemperature = MinTemperature;
        }
    }
}
