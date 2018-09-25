using Messages.WebMessages;
using System.Collections.Generic;

namespace Domotique.Model
{
    public interface IDataRead
    {
        int ReadRoomIdByRoomName(string RoomName);
        string ReadRoomNameByProbe(string CaptorId);

        Room ReadRoomByName(string RoomName);

        IList<DeviceStatus> ReadDevices();

        IList<RoomStatus> ReadRoomTemperatures();

        IList<Graph> ReadRoomTemperaturesGraph();
    }
}