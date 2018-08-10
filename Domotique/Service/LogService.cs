using Domotique.Model;
using MySql.Data.MySqlClient;
using System;

namespace Domotique.Service.Log
{
    public class LogService : ILogService
    {

        IDataRead _dataRead;

        public LogService(IDataRead dataRead)
        {
            _dataRead = dataRead;
        }


        public void LogTemperatureService(String name, Double currentTemperature, Double? targetTemperature, DateTime logDate)
        {
            int RoomId = _dataRead.ReadRoomIdByRoomName(name);
            using (var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile"))
            {                
                connection.Open();
               using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO `DomotiqueCore`.`TemperatureLog` (`CurrentTemp`,`LogDate`,`RoomId`,`TargetTemp`) VALUES" +
                                    "(@CurrentTemp,@LogDate,@RoomId,@TargetTemp);";
                    command.Parameters.AddWithValue("@CurrentTemp", currentTemperature);
                    command.Parameters.AddWithValue("@LogDate", logDate);
                    command.Parameters.AddWithValue("@RoomId", RoomId);
                    command.Parameters.AddWithValue("@TargetTemp", targetTemperature);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Stored into DB: {currentTemperature}° for {name} at {logDate}");
                }
            }
        }
    }
}
