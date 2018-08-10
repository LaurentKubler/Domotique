using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{/*
CREATE TABLE `Schedule` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18303 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;*/
    public class Schedule
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }        
    }
}
