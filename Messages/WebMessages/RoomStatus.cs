using System;
using System.Collections.Generic;

namespace Messages.WebMessages
{
    public class RoomStatus
    {
        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public int Picture { get; set; }

        public int? HeaterID { get; set; }

        public bool HeatRegulation { get; set; }

        public IList<DayTemperature> Temperatures { get; set; }

        public DateTime LastTemperatureRefresh { get; set; }

        public float CurrentTemperature { get; set; }
    }
}
