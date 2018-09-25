namespace Messages.WebMessages
{
    public class DeviceStatus
    {
        public long Device_ID { get; set; }

        public string DeviceName { get; set; }

        public long Value { get; set; }

        public bool? Status { get; set; }

        public string OnImage_ID { get; set; }

        public string OffImage_ID { get; set; }
    }
}
