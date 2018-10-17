using System.ComponentModel.DataAnnotations.Schema;

namespace Domotique.Database
{
    [Table("Room")]
    public class Room
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        [Column("Captor")]
        [ForeignKey("Device")]
        public int CaptorID { get; set; }

        public Device Captor { get; set; }
    }
}
