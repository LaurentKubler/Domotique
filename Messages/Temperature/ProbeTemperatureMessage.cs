﻿using System;

namespace Messages
{
    public class ProbeTemperatureMessage : GenericMessage
    {

        public ProbeTemperatureMessage() : base("ProbeTemperatureMessage")
        {
            
        }

        public float TemperatureValue { get; set; }

        public String ProbeAddress { get; set; }

    }
}
