namespace Messages.WebMessages
{
    public class DeviceStatus
    {
        public long Device_ID { get; set; }

        public string DeviceName { get; set; }

        public long Value { get; set; }

        public bool? Status { get; set; }

        public long OnImage_ID { get; set; }

        public long OffImage_ID { get; set; }
    }
}
