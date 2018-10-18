using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domotique.Database
{/*
CREATE TABLE `Schedule` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18303 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;*/
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
