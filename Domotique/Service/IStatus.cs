using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    public interface IStatusService
    {

        void RegisterTemperature(String RoomName, double Temperature, DateTime date);
    }
}
