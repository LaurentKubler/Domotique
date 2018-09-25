using System;

namespace Messages.Queue.Model
{
    public class RoomTemperatureMessage : GenericMessage
    {
        public RoomTemperatureMessage() : base("RoomTemperatureMessage")
        {

        }

        public float TemperatureValue { get; set; }

        public float TemperatureTargetValue { get; set; }

        public String RoomName { get; set; }

    }
}
