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
            using (var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueDev;uid=laurent;password=odile"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO `DomotiqueDev`.`TemperatureLog` (`CurrentTemp`,`LogDate`,`Room`,`TargetTemp`) VALUES" +
                                    "(@CurrentTemp,@LogDate,@Room,@TargetTemp);";
                    command.Parameters.AddWithValue("@CurrentTemp", currentTemperature);
                    command.Parameters.AddWithValue("@LogDate", logDate);
                    command.Parameters.AddWithValue("@Room", name);
                    command.Parameters.AddWithValue("@TargetTemp", targetTemperature);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
