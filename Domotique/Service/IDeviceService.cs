using Messages.WebMessages;
using System.Collections.Generic;

namespace Domotique.Service
{
    public interface IDeviceService
    {
        void PowerOff(long deviceID);

        void PowerOn(long deviceID);

        void PowerOn(long deviceID, long value);

        List<DeviceStatus> GetDeviceStatus();
    }
}
