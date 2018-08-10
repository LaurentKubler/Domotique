using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
   
CREATE TABLE `TemperatureLog` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `CurrentTemp` float DEFAULT NULL,
  `LogDate` datetime DEFAULT NULL,
  `Room` varchar(255) DEFAULT NULL,
  `TargetTemp` float DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `TargetTemp` (`TargetTemp`),
  KEY `Room` (`Room`),
  KEY `LogDate` (`Room`,`LogDate`)
) ENGINE=MyISAM AUTO_INCREMENT=1986215 DEFAULT CHARSET=latin1;*/
    public class TemperatureLog
    {
        [Key]
        public int ID { get; set; }

        public float CurrentTemp { get; set; }

        public DateTime LogDate { get; set; }

        public string Room { get; set; }

        public float TargetTemp { get; set; }

    }
}
