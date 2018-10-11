namespace Messages.Queue.Model
{
    public class CommandMessage : GenericMessage
    {
        public CommandMessage() : base("CommandMessage")
        {

        }

        public string TargetAddress { get; set; }

        public string TargetAdapter { get; set; }

        public string Command { get; set; }

        public float Paramter { get; set; }

    }
}
