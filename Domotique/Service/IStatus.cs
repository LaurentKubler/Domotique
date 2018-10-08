using Messages.Queue.Model;
using System;

namespace Domotique.Service
{
    public interface IStatusService
    {
        void RegisterTemperature(string RoomName, double Temperature, DateTime date);


        void RegisterDeviceStatus(DeviceStatusMessage deviceStatusMessage);
    }
}
