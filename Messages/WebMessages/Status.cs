using System.Collections.Generic;

namespace Messages.WebMessages
{
    public class Status
    {
        public IList<RoomStatus> Rooms { get; set; } = new List<RoomStatus>();


        public IList<DeviceStatus> Devices { get; set; } = new List<DeviceStatus>();


        public string Version { get; set; }
    }
}
