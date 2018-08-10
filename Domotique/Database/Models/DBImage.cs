using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domotique.Database
{
    /*CREATE TABLE `DBImage` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `DATA` longblob,
  `NAME` varchar(255) DEFAULT NULL,
  PRIMARY KEY(`ID`)
) ENGINE=InnoDB AUTO_INCREMENT = 18407 DEFAULT CHARSET = latin1;*/
    public class DBImage
    {
        [Key]
        public int ID { get; set; }

        //public string Data { get; set; }
        public string Name { get; set; }
    }
}
