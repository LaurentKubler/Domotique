using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.WebMessages
{
    public class Status
    {
        public IList<RoomStatus> Rooms { get; set; }
    }


    public class RoomStatus
    {
        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public int Picture { get; set; }

        public IList<DayTemperature> Temperatures { get; set; }

        public DateTime LastTemperatureRefresh { get; set; }

        public float currentTemperature { get; set; }
    }


    public class DayTemperature
    {
        public DateTime TemperatureDate { get; set; }

        public float MinTemp { get; set; }

        public float MaxTemp { get; set; }

    }


    public class Graph
    {
        public string Name { get; set; }

        public List<List<object>> Data { get; set; }
    }
}
