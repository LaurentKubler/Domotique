namespace Messages.Queue.Model
{
    public class DeviceStatusMessage : GenericMessage
    {
        public DeviceStatusMessage() : base("DeviceMessage")
        {

        }

        public string DeviceAddress { get; set; }

        public string DeviceAdapter { get; set; }

        public string Value { get; set; }


        public override string ToString()
        {
            return ($"Address : {DeviceAddress}/Adapter : {DeviceAdapter}/Value : {Value}/message date : {MessageDate.ToString()}");
        }
    }
}
