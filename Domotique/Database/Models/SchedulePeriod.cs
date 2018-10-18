using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domotique.Database
{
    public class SchedulePeriod
    {
        [Key]
        public int SchedulePeriodID { get; set; }

        [ForeignKey("Schedule")]
        public int ScheduleID { get; set; }

        public string DayOfWeek { get; set; }

        public float StartHour { get; set; }

        //  public int StartMinute { get; set; }

        public float Duration { get; set; }

        public bool IsActiveOn(DateTime time)
        {
            return false;
        }

    }
}
