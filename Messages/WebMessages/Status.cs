using System.Collections.Generic;

namespace Messages.WebMessages
{
    public class Status
    {
        public IList<RoomStatus> Rooms { get; set; }

        public IList<DeviceStatus> Devices { get; set; }
    }
}
