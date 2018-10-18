using Messages.WebMessages;
using System.Collections.Generic;

namespace Domotique.Model
{
    public interface IDataRead
    {
        IList<DeviceStatus> ReadDevices();

        IList<RoomStatus> ReadRoomTemperatures();

        IList<Graph> ReadRoomTemperaturesGraph();
    }
}