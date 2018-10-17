using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Domotique.Database
{
    [Table("Functions")]
    public class Function
    {
        [Key]
        public int ID { get; set; }

        [Column("Function")]
        public string Name { get; set; }

        public int DeviceID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Device Device { get; set; }
    }
}
