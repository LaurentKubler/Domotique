using Domotique.Model;
using Domotique.Service.Log;
using System;
using System.Collections.Generic;

namespace Domotique.Service
{
    public class Status : IStatusService
    {
        readonly ITemperatureReadingService TempReadingService;

        readonly Dictionary<string, Room> Rooms;

        readonly ILogService LogService;


        IDataRead _dataRead;


        public Status(ITemperatureReadingService tempReadingService, ILogService logService, IDataRead dataRead)
        {
            Rooms = new Dictionary<string, Room>();
            LogService = logService;
            _dataRead = dataRead;

            TempReadingService = tempReadingService;
            TempReadingService.SetStatusService(this);
            TempReadingService.Start();
        }



        public void RegisterTemperature(string RoomName, double Temperature, DateTime date)
        {
            if (!Rooms.ContainsKey(RoomName))
            {
                Rooms.Add(RoomName, _dataRead.ReadRoomByName(RoomName));
            }

            Room room = Rooms[RoomName];

            //update the roomValues:
            room.CurrentTemperature = Temperature;
            room.LastTemperatureRefreshDate = date;
            // Compute desired temperature
            room.ComputeTemperature();


            // store Temperature in database for the room
            LogService.LogTemperatureService(RoomName, Temperature, room.TargetTemperature, date);

            // Eventually issue command
            if (room.CurrentTemperature < room.TargetTemperature)
            {

            }
            else
            {

            }
        }
    }
}
