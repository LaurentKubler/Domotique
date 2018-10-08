namespace Messages.Queue.Model
{
    public class DeviceStatusMessage : GenericMessage
    {
        public DeviceStatusMessage() : base("DeviceMessage")
        {

        }

        public string DeviceAddress { get; set; }

        public string DeviceAdapter { get; set; }

        public int Value { get; set; }

    }
}
