using Domotique.Database;
using Messages.WebMessages;
using System.Collections.Generic;

namespace Domotique.Model
{
    public interface IDataRead
    {
        //  int ReadRoomIdByRoomName(string RoomName);

        //string ReadRoomNameByProbe(string CaptorId);

        //  int ReadDeviceIDByAddress(string address, string adapter);

        // Device ReadDeviceByID(long deviceID);

        Room ReadRoomByName(string RoomName);

        IList<DeviceStatus> ReadDevices();

        IList<RoomStatus> ReadRoomTemperatures();

        IList<Graph> ReadRoomTemperaturesGraph();


    }
}