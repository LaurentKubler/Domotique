using Domotique.Database;
using Domotique.Model;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace Domotique.Service.Log
{
    public class LogService : ILogService
    {

        IDataRead _dataRead;

        DBContextProvider _provider;

        public LogService(IDataRead dataRead, DBContextProvider provider)
        {
            _dataRead = dataRead;
            _provider = provider;
        }


        public void LogTemperatureService(string name, double currentTemperature, double? targetTemperature, DateTime logDate)
        {
            using (var _context = _provider.getContext())
            {
                int RoomID = _context.Rooms.Where(room => room.Name == name).First().ID;
                var tempLog = new TemperatureLog()
                {
                    CurrentTemp = currentTemperature,
                    LogDate = logDate,
                    RoomID = RoomID,
                    TargetTemp = targetTemperature ?? 0
                };

                _context.Add(tempLog);
                _context.SaveChanges();
                Console.WriteLine($"Stored into DB: {currentTemperature}° for {name} at {logDate}");
            }
        }


        public void LogDeviceStatus(int deviceID, int deviceValue, DateTime valueDate)
        {
            using (var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO `DomotiqueCore`.`DeviceStatus` (`Device_ID`, `DeviceValue`, `ValueDate`, `ValueRequest`) VALUES" +
                                    "(@Device_ID, @DeviceValue, @ValueDate, @ValueRequest);";
                    command.Parameters.AddWithValue("@Device_ID", deviceID);
                    command.Parameters.AddWithValue("@DeviceValue", deviceValue);
                    command.Parameters.AddWithValue("@ValueDate", valueDate);
                    command.Parameters.AddWithValue("@ValueRequest", null);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Stored into DB: {deviceValue}° for {deviceID} at {valueDate}");
                }
            }
        }
    }
}
