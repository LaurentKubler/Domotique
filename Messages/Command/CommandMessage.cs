using System;

namespace Messages
{
    public class CommandMessage : GenericMessage
    {
        public CommandMessage() : base("CommandMessage")
        {

        }

        public string TargetAddress { get; set; }

        public String TargetAdapter { get; set; }

        public String Command { get; set; }

        public float Paramter { get; set; }

    }
}
