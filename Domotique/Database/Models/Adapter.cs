using System.ComponentModel.DataAnnotations;

namespace Domotique.Database
{
    public class Adapter
    {
        [Key]
        public int AdapterID { get; set; }

        public string AdapterName { get; set; }
        public string IPAddress { get; set; }
        public int IPPort { get; set; }
        public string DeviceType { get; set; }
        public int ExpirationTime { get; set; }
        public long AutoRefreshDelay { get; set; }
        public bool Enabled { get; set; }


        //        public List<Post> Posts { get; set; }
    }
}
