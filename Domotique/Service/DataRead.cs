using Domotique.Controllers;
using Domotique.Service;
using Messages.WebMessages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using static Domotique.Controllers.StatusController;

namespace Domotique.Model
{
    class DataRead : IDataRead
    {

        IDatabaseConnection _databaseConnection;


        public DataRead(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }



        public IList<RoomStatus> ReadRoomTemperatures()
        {
            IList<RoomStatus> result = new List<RoomStatus>();

            Dictionary<int, RoomStatus> rooms = new Dictionary<int, RoomStatus>();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select * from Room;";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        RoomStatus room = new RoomStatus()
                        {
                            RoomId = reader.GetInt32("ID"),
                            RoomName = reader.GetString("Name"),
                            //  Picture = reader.GetInt32("Picture")
                        };
                        rooms.Add(room.RoomId, room);
                    }
                    reader.Close();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select  Room.Id, Name, min(CurrentTemp) Min, max(CurrentTemp) Max,DATE(LogDate) " +
                        "from TemperatureLog " +
                        "inner join Room on Room.ID = TemperatureLog.RoomId " +
                        "where  LogDate >  date_sub(now(),INTERVAL 1 WEEK) group by  RoomId, DATE(LogDate);";


                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var roomId = reader.GetInt32(0);
                            var roomName = reader.GetString(1);
                            var roomMin = reader.GetFloat(2);
                            var roomMax = reader.GetFloat(3);
                            var roomLastRefresh = reader.GetDateTime(4);
                            if (rooms[roomId].Temperatures == null)
                                rooms[roomId].Temperatures = new List<DayTemperature>();
                            rooms[roomId].Temperatures.Add(new DayTemperature()
                            {
                                MaxTemp = reader.GetFloat(3),
                                MinTemp = reader.GetFloat(2),
                                TemperatureDate = reader.GetDateTime(4)
                            });
                        }

                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        " select   " +
                        "   l.RoomId, l.CurrentTemp, l.LogDate " +
                        "from TemperatureLog l " +
                        "where " +
                        "   row(l.LogDate, l.roomId) in   ( select max(LogDate), RoomId from TemperatureLog group by RoomId); ";


                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var roomId = reader.GetInt32(0);
                            var roomLastRefresh = reader.GetDateTime(2);
                            rooms[roomId].LastTemperatureRefresh = reader.GetDateTime(2);
                            rooms[roomId].CurrentTemperature = reader.GetFloat(1);
                        }

                    }
                }

                return rooms.Values.ToList();
            }
        }


        public IList<Graph> ReadRoomTemperaturesGraph()
        {
            IList<Graph> result = new List<Graph>();

            Dictionary<int, RoomStatus> rooms = new Dictionary<int, RoomStatus>();
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select * from Room;";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        RoomStatus room = new RoomStatus()
                        {
                            RoomId = reader.GetInt32("ID"),
                            RoomName = reader.GetString("Name"),
                            //  Picture = reader.GetInt32("Picture")
                        };
                        rooms.Add(room.RoomId, room);
                    }
                    reader.Close();
                }

                using (var command = connection.CreateCommand())
                {
                    foreach (var room in rooms.Values)
                    {
                        command.CommandText = "SELECT CurrentTemp, LogDate " +
                            "FROM TemperatureLog " +
                            "INNER JOIN Room ON Room.ID = TemperatureLog.RoomId " +
                            $"WHERE LogDate >  date_sub(now(),INTERVAL 1 WEEK) and  TemperatureLog.RoomId  = {room.RoomId}";


                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            var graph = new Graph()
                            {
                                Name = room.RoomName,
                                Data = new List<List<object>>()
                            };
                            while (reader.Read())
                            {
                                var list = new List<object>
                            {
                                (reader.GetDateTime(1).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds * 1000,
                                //list.Add(reader.GetDateTime(1).ToUniversalTime().to);
                                reader.GetDouble(0)
                            };
                                graph.Data.Add(list);
                            }
                            var liste = new List<Point>();
                            List<Point> points = graph.Data.Select(c => new Point() { X = (double)c[0], Y = ((double)c[1]) }).ToList();
                            List<Point> pointsreduced = DouglasPeuckerReduction(points, 0.1);

                            graph.Data = pointsreduced
                                .Select(p => { var list = new List<object> { p.X, p.Y }; return list; })
                                .ToList();

                            result.Add(graph);
                        }
                    }
                }

                return result.ToList();
            }
        }


        public IList<DeviceStatus> ReadDevices()
        {
            IList<DeviceStatus> result = new List<DeviceStatus>();

            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Device.*, DeviceStatus.ValueDate,DeviceStatus.DeviceValue" +
                                          "    FROM Device" +
                                          "    LEFT JOIN(SELECT Device_ID, MAX(DeviceStatusID) MaxStatus FROM DeviceStatus GROUP BY Device_ID) MaxDateDeviceStatus" +
                                          "          ON MaxDateDeviceStatus.Device_ID = Device.DeviceID" +
                                          "    LEFT JOIN DeviceStatus" +
                                          "          ON DeviceStatus.Device_ID = Device.DeviceID AND DeviceStatus.DeviceStatusID = MaxDateDeviceStatus.MaxStatus";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DeviceStatus device = new DeviceStatus()
                        {
                            Device_ID = reader.GetInt32("DeviceID"),
                            DeviceName = reader.GetString("DeviceName"),
                            OnImage_ID = reader.GetInt32("OnImage"),
                            OffImage_ID = reader.GetInt32("OffImage")
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ValueDate")))
                            device.ValueDate = reader.GetDateTime("ValueDate");
                        else
                            device.ValueDate = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("DeviceValue")))
                            device.Value = reader.GetInt32("DeviceValue");
                        else
                            device.Value = null;

                        if (device.Value != null)
                            device.Status = (device.Value != 0 ? true : false);
                        result.Add(device);
                    }
                    reader.Close();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * from Functions";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var func = new Messages.WebMessages.Function()
                        {
                            Name = reader.GetString("Function")
                        };

                        int id = reader.GetInt32("DeviceID");

                        result.First(device => device.Device_ID == id).Functions.Add(func);
                    }
                    reader.Close();
                }
            }
            return result;
        }
    }

}
