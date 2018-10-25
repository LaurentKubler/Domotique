using Domotique.Controllers;
using Domotique.Database;
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

        IDBContextProvider _provider;

        DomotiqueContext _context;

        public DataRead(IDatabaseConnection databaseConnection, IDBContextProvider provider)
        {
            _databaseConnection = databaseConnection;
            _provider = provider;
            _context = provider.getContext();
        }



        public IList<RoomStatus> ReadRoomTemperatures()
        {
            IList<RoomStatus> result = new List<RoomStatus>();

            /*      var query = from room in _context.Rooms
                              from temperatureLog in _context.TemperatureLog
                              .Where(tl => tl.RoomID == room.ID && tl.LogDate ==
                                  _context.TemperatureLog.Where(tl1 => tl1.RoomID == room.ID).Max(o1 => o1.LogDate)
                              )
                              select new RoomStatus()
                              {
                                  RoomId = room.ID,
                                  RoomName = room.Name,
                                  HeaterID = room.HeaterID,
                                  HeatRegulation = room.HeatRegulation,
                                  LastTemperatureRefresh = temperatureLog.LogDate,
                                  CurrentTemperature = (float)temperatureLog.CurrentTemp
                              };
                  Console.Write(query.Count());*/
            //Dictionary<int, RoomStatus> rooms = new Dictionary<int, RoomStatus>();
            using (var connection = _databaseConnection.GetConnection())
            {
                var rooms = _context.Rooms.Select(r => new RoomStatus()
                {
                    RoomId = r.ID,
                    RoomName = r.Name,
                    HeaterID = r.HeaterID,
                    HeatRegulation = r.HeatRegulation
                }).ToList();

                connection.Open();

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
                            foreach (var room in rooms.Where(r => r.RoomId == roomId))
                            {
                                if (room.Temperatures == null)
                                    room.Temperatures = new List<DayTemperature>();
                                room.Temperatures.Add(new DayTemperature()
                                {
                                    MaxTemp = reader.GetFloat(3),
                                    MinTemp = reader.GetFloat(2),
                                    TemperatureDate = reader.GetDateTime(4)
                                });
                            };
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
                            foreach (var room in rooms.Where(r => r.RoomId == roomId))
                            {
                                room.LastTemperatureRefresh = reader.GetDateTime(2);
                                room.CurrentTemperature = reader.GetFloat(1);
                            };
                        }

                    }
                }

                return rooms.ToList();
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


        public IList<Messages.WebMessages.DeviceStatus> ReadDevices()
        {
            //List<Messages.WebMessages.DeviceStatus> list = new List<Messages.WebMessages.DeviceStatus>();

            /*  var dev = from device in _context.Device//.Include(d => d.Functions)
                        join devicesstatus in _context.DeviceStatus
                           on _context.DeviceStatus.Where(statusmax => statusmax.Device_ID == device.DeviceID).Max(o1 => o1.DeviceStatusID) equals devicesstatus.DeviceStatusID
                        select new Messages.WebMessages.DeviceStatus()
                        {
                            DeviceName = device.DeviceName,
                            Device_ID = device.DeviceID,
                            Status = (devicesstatus.DeviceValue != 0),
                            Value = devicesstatus.DeviceValue,
                            ValueDate = (devicesstatus.ValueDate),
                            /*Functions = device.Functions.Select(c => new Messages.WebMessages.Function()
                            {
                                Name = c.Name
                            }).ToList()
                        };

              /*var query = from device in _context.Device.Include(device => device.Functions)
                          from devicesstatus in _context.DeviceStatus
                          where (devicesstatus.ValueDate == _context.DeviceStatus.Where(statusmax => statusmax.Device_ID == device.DeviceID).Max(o1 => o1.ValueDate))
                              && devicesstatus.Device_ID == device.DeviceID
                          select new Messages.WebMessages.DeviceStatus()
                          {
                              DeviceName = device.DeviceName,
                              Device_ID = device.DeviceID,
                              Status = (devicesstatus.DeviceValue != 0),
                              Value = devicesstatus.DeviceValue,
                              ValueDate = (devicesstatus.ValueDate),
                              Functions = device.Functions.Select(c => new Messages.WebMessages.Function()
                              {
                                  Name = c.Name
                              }).ToList()
                          };*/
            /*var test = from device in _context.Device.Include(device => device.Functions)
                           //from function in _context.Function.Where(func => func.)
                       from devicestatus in _context.DeviceStatus
                       where (device.DeviceID == devicestatus.Device_ID
                           && devicestatus.ValueDate == _context.DeviceStatus.Where(tl1 => tl1.Device_ID == devicestatus.Device_ID).Max(o1 => o1.ValueDate))
                       select device.DeviceID, devicestatus.Device_ID;*/
            /*
             *  var query = from device in _context.Device.Include(device => device.Functions)
                            //from function in _context.Function.Where(func => func.)
                        from devicestatus in _context.DeviceStatus
                        .Where(ds => ds.Device_ID == device.DeviceID && ds.ValueDate ==
                            _context.DeviceStatus.Where(tl1 => tl1.Device_ID == device.DeviceID).Max(o1 => o1.ValueDate)
                        )
                        select new Messages.WebMessages.DeviceStatus()
                        {
                            DeviceName = device.DeviceName,
                            Device_ID = device.DeviceID,
                            Status = (devicestatus.DeviceValue != 0),
                            Value = devicestatus.DeviceValue,
                            ValueDate = (devicestatus.ValueDate),
                            Functions = device.Functions.Select(c => new Messages.WebMessages.Function()
                            {
                                Name = c.Name
                            }).ToList()
                        };
*/
            IList<Messages.WebMessages.DeviceStatus> result = new List<Messages.WebMessages.DeviceStatus>();

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
                        Messages.WebMessages.DeviceStatus device = new Messages.WebMessages.DeviceStatus()
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

            return result.ToList();
        }
        /*
        public IList<DeviceStatus> ReadDevices_old()
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
        }*/
    }

}
