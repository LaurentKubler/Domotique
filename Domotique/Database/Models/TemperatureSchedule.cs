using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
   
CREATE TABLE `TemperatureSchedule` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `PRIORITY` int(11) DEFAULT NULL,
  `TARGETTEMPERATURE` float DEFAULT NULL,
  `SCHEDULE_ID` bigint(20) DEFAULT NULL,
  `TEMPERATUREPLAN_ID` bigint(255) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `FK_TEMPERATURESCHEDULE_SCHEDULE_ID` (`SCHEDULE_ID`),
  KEY `FK_TEMPERATURESCHEDULE_TEMPERATUREPLAN_PIECE` (`TEMPERATUREPLAN_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=latin1;*/
    public class TemperatureSchedule
    {
        [Key]
        public int ID { get; set; }

        public int Priority { get; set; }
        public float TargetTemperature { get; set; }
        public int Schedule_ID { get; set; }
        public int TemperaturePlan_ID { get; set; }
       
    }
}
