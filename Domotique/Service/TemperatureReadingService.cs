using Domotique.Database;
using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using Microsoft.Extensions.Logging;
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

        ILogger<TemperatureReadingService> _logger;

        private IDBContextProvider _provider;

        public TemperatureReadingService(IDataRead dataRead, IQueueConnectionFactory queueConnectionFactory, IDBContextProvider provider, ILogger<TemperatureReadingService> logger)
        {
            _dataRead = dataRead;
            _provider = provider;
            _queueConnectionFactory = queueConnectionFactory;
            _logger = logger;
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

                    _logger.LogInformation($"Stored into DB: { message.TemperatureValue}° for { room.Name} at {message.MessageDate}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.ToString()}: {ex.Message}");
            }

        }
    }
}
