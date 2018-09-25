namespace Messages.Queue.Model
{
    public class DeviceChangeMessage : GenericMessage
    {
        public DeviceChangeMessage() : base("DeviceChangeMessage")
        {

        }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string DeviceName { get; set; }
    }
}
