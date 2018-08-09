﻿using System;

namespace Messages
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
