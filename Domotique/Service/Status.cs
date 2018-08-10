using Domotique.Model;
using Domotique.Service.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    public class Status : IStatusService
    {
        readonly ITemperatureReadingService TempReadingService;

        readonly Dictionary<String, Room> Rooms;

        readonly LogService LogService;

        public Status(ITemperatureReadingService tempReadingService, LogService logService)
        {
            Rooms = new Dictionary<String, Room>();
            LogService = logService;

            TempReadingService = tempReadingService;
            TempReadingService.SetStatusService(this);
            TempReadingService.Start();
        }



        public void RegisterTemperature(String RoomName, double Temperature, DateTime date)
        {            
            if (!Rooms.ContainsKey(RoomName))
            {
                // Read room in database
            }

            Room room = Rooms[RoomName];
            
            //update the roomValues:
            room.CurrentTemperature = Temperature;
            // Compute desired temperature
            room.computeTemperature();


            // store Temperature in database for the room
            LogService.LogTemperatureService(RoomName, Temperature, room.TargetTemperature, date);

            // Eventually issue command
            if (room.CurrentTemperature<room.TargetTemperature)
            {

            }
            else
            {

            }
        }
    }
}
