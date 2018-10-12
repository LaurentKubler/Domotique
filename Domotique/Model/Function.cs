namespace Domotique.Model
{
    public class Function
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int DeviceID { get; set; }

        public Device Device { get; set; }
    }
}
