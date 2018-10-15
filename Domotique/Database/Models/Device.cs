using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Domotique.Database
{
    /*
CREATE TABLE `DomoTrigger` (
  `name` varchar(255) NOT NULL,
  `periodicity` varchar(255) DEFAULT NULL,
  `TriggerStatus` varchar(255) DEFAULT NULL,
  `type` varchar(255) DEFAULT NULL,
  `scenario_id` bigint(20) DEFAULT NULL,
  `source` varchar(255) DEFAULT NULL,
  `sourcePos` varchar(255) DEFAULT NULL,
  PRIMARY KEY(`name`),
  KEY `FKC8692D6B1F57B9A3` (`scenario_id`),
  KEY `FKC8692D6B7AAA0515` (`scenario_id`),
  CONSTRAINT `FK_DomoTrigger_SCENARIO_ID` FOREIGN KEY (`scenario_id`) REFERENCES `ScenarioTemplate` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;*/

    public class Device
    {
        [Key]
        public int DeviceID { get; set; }

        public string DeviceName { get; set; }

        [Column("AdapterID")]
        public int AdapterID { get; set; }
        public Adapter Adapter { get; set; }

        public string Address { get; set; }

        public string StatusAddress { get; set; }

        public IList<Function> Functions { get; set; }


    }
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
    public class Room
    {
        public string Name { get; set; }

        public double? CurrentTemperature { get; set; }

        public double? TargetTemperature { get; set; }

        public DateTime? LastTemperatureRefreshDate { get; set; }

        bool ContainsHeater { get; set; }

        string ProbeName { get; set; }

        string HeaterName { get; set; }

        IList<Schedule> TemperatureSchedule { get; set; }


        public void ComputeTemperature()
        {

        }
    }
}
