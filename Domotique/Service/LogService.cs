using Domotique.Model.Logs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service.Log
{
    public class LogService : ILogService
    {

        public void LogTemperatureService(String name, Double currentTemperature, Double? targetTemperature, DateTime logDate)
        {
            using (var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile"))
            {
                int RoomId = 0;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ID from Room Where Name = @RoomName";
                    command.Parameters.AddWithValue("@RoomName", name);
                    RoomId = Int32.Parse(command.ExecuteScalar().ToString());
                }
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
