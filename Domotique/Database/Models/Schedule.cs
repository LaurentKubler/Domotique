using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domotique.Database
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }

        public string Name { get; set; }

        public IList<SchedulePeriod> Periods { get; set; }


        public bool IsActiveOn(DateTime time)
        {
            foreach (var period in Periods)
            {
                if (period.IsActiveOn(time))
                    return true;
            }
            return false;
        }
    }
}
