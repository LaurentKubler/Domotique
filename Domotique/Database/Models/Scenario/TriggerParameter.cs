using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
  
CREATE TABLE `TriggerParameter` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `parameterName` varchar(255) DEFAULT NULL,
  `parameterValue` varchar(255) DEFAULT NULL,
  `trigger_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `FK2D9AB0F167C51C89` (`trigger_name`),
  CONSTRAINT `FK_TriggerParameter_TRIGGER_NAME` FOREIGN KEY (`trigger_name`) REFERENCES `DomoTrigger` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=latin1;
*/
    public class TriggerParameter
    {
        [Key]
        public int ID { get; set; }

        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string Trigger_name { get; set; }
       
    }
}
