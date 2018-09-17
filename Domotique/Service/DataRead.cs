using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Messages.WebMessages;
using static Domotique.Controllers.StatusController;
using Domotique.Controllers;
using Domotique.Service;

namespace Domotique.Model
{
    class DataRead : IDataRead
    {

        IDatabaseConnection _databaseConnection;


        public DataRead(IDatabaseConnection databaseConnectio)
        {

        }


        public String ReadRoomNameByProbe(String CaptorId)
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select Name from Room left join Device on Device.DeviceID = Room.Captor where Device.Address = @CaptorId";
                    command.Parameters.AddWithValue("@CaptorId", CaptorId.Replace("/", String.Empty));
                    var name = command.ExecuteScalar().ToString();
                    return name;
                }
            }
        }


        public int ReadRoomIdByRoomName(String RoomName)
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                //var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile");
                int RoomId = 0;
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ID from Room Where Name = @RoomName";
                    command.Parameters.AddWithValue("@RoomName", RoomName);
                    RoomId = Int32.Parse(command.ExecuteScalar().ToString());
                }
                return RoomId;
            }
        }


        public Room ReadRoomByName(String RoomName)
        {
            using (var connection = _databaseConnection.GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "Select * from Room where Name = @RoomName";
                command.Parameters.AddWithValue("@RoomName", RoomName);
                var roomLine = command.ExecuteReader().Read();
                Room newRoom = new Room()
                {
                    Name = RoomName,
                    TargetTemperature = 0
                };
                return newRoom;
            }
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
    }
}
