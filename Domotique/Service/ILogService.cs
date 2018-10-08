using System;

namespace Domotique.Service.Log
{
    public interface ILogService
    {
        void LogTemperatureService(string name, double currentTemperature, double? targetTemperature, DateTime logDate);

        void LogDeviceStatus(int deviceID, int deviceValue, DateTime valueDate);
    }
}