using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{
    public class Adapter
    {
        [Key]
        public int ID { get; set; }

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
