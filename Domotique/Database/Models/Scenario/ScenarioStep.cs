using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
CREATE TABLE `ScenarioStep` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `actio` varchar(255) DEFAULT NULL,
  `scenarioTemplate_id` bigint(20) DEFAULT NULL,
  `stepNumber` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FKCDB3FA9C2A7F72E9` (`scenarioTemplate_id`),
  KEY `FKCDB3FA9C85D1BE5B` (`scenarioTemplate_id`),
  CONSTRAINT `FK_ScenarioStep_SCENARIOTEMPLATE_ID` FOREIGN KEY (`scenarioTemplate_id`) REFERENCES `ScenarioTemplate` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=latin1;
CREATE TABLE `ScenarioStepParameters` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ScenarioStep_id` bigint(20) DEFAULT NULL,
  `ParameterName` varchar(255) DEFAULT NULL,
  `ParameterValue` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ScenarioStep_id` (`ScenarioStep_id`),
  CONSTRAINT `FK_ScenarioStepParameters_SCENARIOSTEP_ID` FOREIGN KEY (`ScenarioStep_id`) REFERENCES `ScenarioStep` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;*/
    public class ScenarioStep
    {
        [Key]
        public int ID { get; set; }

        public string Actio { get; set; }

        public int ScenarioTemplate_id { get; set; }

        public string StepNumber { get; set; }
    }


    public class ScenarioStepParameters
    {
        [Key]
        public int ID { get; set; }

        public int  ScenarioStep_id { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }
    }

}
