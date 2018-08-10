using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
   
CREATE TABLE `TemperaturePlan` (
  `ID` bigint(20) NOT NULL,
  `PIECE` varchar(255) NOT NULL,
  `COMMENT` varchar(255) DEFAULT NULL,
  `Sonde` varchar(64) NOT NULL,
  `Chauffage` bigint(64) DEFAULT NULL,
  `DefaultTemp` float NOT NULL,
  `Active` int(11) NOT NULL,
  `ExceptionTemp` float NOT NULL,
  PRIMARY KEY (`PIECE`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;*/
    public class TemperaturePlan
    {
        [Key]
        public int ID { get; set; }

        public string Piece { get; set; }

        public string Comment { get; set; }

        [Column("Sonde")]
        public int SondeID { get; set; }
        public Device Sonde { get; set; }

        [Column("Chauffage")]
        public int? ChauffageID { get; set; }
        public Device Chauffage { get; set; }

        public float ExceptionTemp { get; set; }

        public float DefaultTemp { get; set; }

        [Column("Active")]
        public bool IsActive { get; set; }
    }
}
