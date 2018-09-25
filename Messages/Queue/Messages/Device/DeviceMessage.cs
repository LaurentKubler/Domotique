using System;

namespace Messages.Queue.Model
{
    public class DeviceMessage : GenericMessage
    {
        public DeviceMessage() : base("DeviceMessage")
        {

        }

        public string TargetAddress { get; set; }

        public String TargetAdapter { get; set; }

        public string Value { get; set; }

    }
}
