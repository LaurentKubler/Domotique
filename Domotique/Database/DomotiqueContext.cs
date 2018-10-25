using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Domotique.Database
{

    public class DomotiqueContext : DbContext
    {


        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public DomotiqueContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<EDayOfWeek>();

            modelBuilder.Entity<SchedulePeriod>()
                        .Property(e => e.DayOfWeek)
                        .HasConversion(converter);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //   optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        public DbSet<Adapter> Adapter { get; set; }

        public DbSet<DBImage> DBImage { get; set; }

        public DbSet<Device> Device { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<DeviceStatus> DeviceStatus { get; set; }
        public DbSet<Function> Functions { get; set; }

        //LK a tester 
        public DbSet<TemperatureLog> TemperatureLog { get; set; }
        /*  public DbSet<DeviceChangeEvent> DeviceChangeEvent { get; set; }

          public DbSet<DeviceRequestLog> DeviceRequestLog { get; set; }*/

        public DbSet<ScenarioStep> ScenarioStep { get; set; }

        public DbSet<ScenarioStepParameters> ScenarioStepParameters { get; set; }

        public DbSet<ScenarioTemplate> ScenarioTemplate { get; set; }

        public DbSet<Schedule> Schedule { get; set; }



        public DbSet<TemperaturePlan> TemperaturePlan { get; set; }

        public DbSet<TemperatureSchedule> TemperatureSchedule { get; set; }

        public DbSet<Trigger> Trigger { get; set; }

        public DbSet<TriggerParameter> TriggerParameter { get; set; }


        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // add your own confguration here
        }*/
    }

}


/*  public class Post
  {
      public int PostId { get; set; }
      public string Title { get; set; }
      public string Content { get; set; }

      public int BlogId { get; set; }
      public Device Device { get; set; }
  }
CREATE DATABASE `DomotiqueDev` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_general_ci ;
 CREATE TABLE `Adapter` (
`id` int (11) NOT NULL AUTO_INCREMENT,
`AdapterName` varchar(256) COLLATE latin1_general_ci NOT NULL,
`IPAddress` varchar(32) COLLATE latin1_general_ci NOT NULL,
`IPPort` int (11) NOT NULL,
`deviceType` varchar(32) COLLATE latin1_general_ci NOT NULL,
`ExpirationTime` bigint(20) NOT NULL,
`AutoRefreshDelay` bigint(20) NOT NULL,
`Enabled` tinyint(1) NOT NULL,
PRIMARY KEY(`id`)
) ENGINE=InnoDB AUTO_INCREMENT = 13 DEFAULT CHARSET = latin1 COLLATE=latin1_general_ci;

 CREATE TABLE `Device` (
`DeviceName` varchar(128) COLLATE latin1_general_ci NOT NULL,
`Adapter` int (11) NOT NULL,
`StatusAddress` varchar(128) COLLATE latin1_general_ci DEFAULT NULL,
`Address` varchar(128) COLLATE latin1_general_ci NOT NULL,
`Picture` varchar(10) COLLATE latin1_general_ci NOT NULL,
`id` int (11) NOT NULL AUTO_INCREMENT,
PRIMARY KEY(`id`)
) ENGINE=InnoDB AUTO_INCREMENT = 17 DEFAULT CHARSET = latin1 COLLATE=latin1_general_ci ROW_FORMAT = DYNAMIC;




}*/
