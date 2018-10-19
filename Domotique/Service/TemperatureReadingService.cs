using Domotique.Database;
using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using System;
using System.Linq;

namespace Domotique.Service
{
    public class TemperatureReadingService : IDisposable, ITemperatureReadingService
    {

        public static String ServerName { get; set; }

        public static String QueueName { get; set; }

        public bool IsStarted { get; set; } = false;


        private IDataRead _dataRead;

        private IQueueConnectionFactory _queueConnectionFactory;

        private IQueueSubscriber<ProbeTemperatureMessage> subscriber;

        //private DbContextOptions<DomotiqueContext> _options;
        private IDBContextProvider _provider;

        public TemperatureReadingService(IDataRead dataRead, IQueueConnectionFactory queueConnectionFactory, IDBContextProvider provider)
        {
            _dataRead = dataRead;
            _provider = provider;
            _queueConnectionFactory = queueConnectionFactory;
        }


        public void Start()
        {
            subscriber = _queueConnectionFactory.GetQueueSubscriber<ProbeTemperatureMessage>("Temperature");
            subscriber.OnMessage += Received;
            subscriber.Connect();
        }

        public void Dispose()
        {
            subscriber.Disconnect();
        }


        private void Received(ProbeTemperatureMessage message)
        {
            try
            {

                using (var dbContext = _provider.getContext())
                {
                    var address = message.ProbeAddress.Replace("/", string.Empty);
                    var room = dbContext.Rooms.Where(c => c.Captor.Address == address).First();

                    if (room.HeatRegulation)
                        room.ComputeTemperature(new DateTime());

                    var tempLog = new TemperatureLog()
                    {
                        CurrentTemp = message.TemperatureValue,
                        LogDate = message.MessageDate,
                        RoomID = room.ID,
                        TargetTemp = room.TargetTemperature ?? 0
                    };

                    dbContext.Add(tempLog);
                    dbContext.SaveChanges();
                    Console.WriteLine($"Stored into DB: { message.TemperatureValue}° for { room.Name} at {message.MessageDate}");


                    /*
                    
                    // Eventually issue command
                    if (room.CurrentTemperature < room.TargetTemperature)
                    {

                    }
                    else
                    {

                    }*/
                }
            }
            catch (Exception ex)
            {
                Console.Write($"Temperature reception: {ex.Message}");
            }

        }
    }
}
