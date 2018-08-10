using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model
{
    class DataRead : IDataRead
    {
        public String ReadRoomNameByProbe(String CaptorId)
        {
            var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "Select Name from Room left join Device on Device.DeviceID = Room.Captor where Device.Address = @CaptorId";
            command.Parameters.AddWithValue("@CaptorId", CaptorId.Replace("/", String.Empty));
            var name = command.ExecuteScalar().ToString();
            return name;
        }


        public int ReadRoomIdByRoomName(String RoomName)
        {

            var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueCore;uid=laurent;password=odile");
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

        public Room ReadRoomByName(String RoomName)
        {
            var connection = new MySqlConnection("server=192.168.1.34;port=3306;database=DomotiqueDev;uid=laurent;password=odile");
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
}
