using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{
    [Table("DomoTrigger")]
    public class Trigger
    {
        [Key]
        public string Name { get; set; }

        public string Periodicity { get; set; }

        public string TriggerStatus { get; set; }

        public string Type { get; set; }

        public int Scenario_ID { get; set; }

        public string Source { get; set; }

        public string SourcePos { get; set; }

    }
}
