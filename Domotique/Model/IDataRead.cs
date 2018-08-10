namespace Domotique.Model
{
    public interface IDataRead
    {
        int ReadRoomIdByRoomName(string RoomName);
        string ReadRoomNameByProbe(string CaptorId);

        Room ReadRoomByName(string RoomName);
    }
}