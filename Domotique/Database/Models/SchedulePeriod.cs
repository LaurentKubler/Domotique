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

        public EDayOfWeek DayOfWeek { get; set; }

        public float StartHour { get; set; }

        //  public int StartMinute { get; set; }

        public float Duration { get; set; }


        public bool IsActiveOn(DateTime time)
        {
            var dayOfWeek = time.DayOfWeek;
            if (!IsValid(DayOfWeek, dayOfWeek))
                return false;
            if (time.Hour < StartHour)
                return false;
            if (time.Hour > StartHour + Duration)
                return false;
            return true;
        }


        private bool IsValid(EDayOfWeek periodday, DayOfWeek dayofweek)
        {
            switch (periodday)
            {
                case EDayOfWeek.EveryDay:
                    return true;
                case EDayOfWeek.WeekEnd:
                    if (dayofweek == System.DayOfWeek.Saturday)
                        return true;
                    if (dayofweek == System.DayOfWeek.Sunday)
                        return true;
                    return false;
                case EDayOfWeek.WorkingDay:
                    if (dayofweek == System.DayOfWeek.Saturday)
                        return false;
                    if (dayofweek == System.DayOfWeek.Sunday)
                        return false;
                    return true;
                case EDayOfWeek.Monday:
                    if (dayofweek == System.DayOfWeek.Monday)
                        return true;
                    return false;
                case EDayOfWeek.Tuesday:
                    if (dayofweek == System.DayOfWeek.Tuesday)
                        return true;
                    return false;
                case EDayOfWeek.Wednesday:
                    if (dayofweek == System.DayOfWeek.Wednesday)
                        return true;
                    return false;
                case EDayOfWeek.Thursday:
                    if (dayofweek == System.DayOfWeek.Thursday)
                        return true;
                    return false;
                case EDayOfWeek.Friday:
                    if (dayofweek == System.DayOfWeek.Friday)
                        return true;
                    return false;
                case EDayOfWeek.Saturday:
                    if (dayofweek == System.DayOfWeek.Saturday)
                        return true;
                    return false;
                case EDayOfWeek.Sunday:
                    if (dayofweek == System.DayOfWeek.Sunday)
                        return true;
                    return false;
            }
            return false;
        }

    }
    public enum EDayOfWeek
    {
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, WorkingDay, WeekEnd, EveryDay

    }
}
