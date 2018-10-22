using Domotique.Database;
using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SignalRChat.Hubs;
using System;
using System.Linq;

namespace Domotique.Service
{
    public class TemperatureReadingService : IDisposable, ITemperatureReadingService
    {

        public static string ServerName { get; set; }

        public static string QueueName { get; set; }

        public bool IsStarted { get; set; } = false;

        private IDataRead _dataRead;

        private IQueueConnectionFactory _queueConnectionFactory;

        private IQueueSubscriber<ProbeTemperatureMessage> subscriber;

        ILogger<TemperatureReadingService> _logger;

        private IDBContextProvider _provider;

        IHubContext<NotificationHub> _notificationHub;


        public TemperatureReadingService(IDataRead dataRead, IQueueConnectionFactory queueConnectionFactory, IDBContextProvider provider, ILogger<TemperatureReadingService> logger, , IHubContext<NotificationHub> context)
        {
            _dataRead = dataRead;
            _provider = provider;
            _queueConnectionFactory = queueConnectionFactory;
            _logger = logger;
            _notificationHub = context;
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
                    var room = dbContext.Rooms.Include(tl => tl.TemperatureSchedules).ThenInclude(t => t.Schedule).ThenInclude(schedule => schedule.Periods).Where(c => c.Captor.Address == address).First();

                    if (room == null)
                    {
                        _logger.LogDebug($"No room found for address '{address}'");
                        return;
                    }

                    if (room.HeatRegulation)
                    {
                        room.ComputeTemperature(new DateTime());
                        _logger.LogInformation($"Computed temperature : {room.TargetTemperature}° /Current Temperature {message.TemperatureValue}° for {room.Name} at {message.MessageDate}");
                    }

                    var tempLog = new TemperatureLog()
                    {
                        CurrentTemp = message.TemperatureValue,
                        LogDate = message.MessageDate,
                        RoomID = room.ID,
                        TargetTemp = room.TargetTemperature ?? 0
                    };

                    dbContext.Add(tempLog);
                    dbContext.SaveChanges();

                    _notificationHub.Clients.All.SendAsync("TemperatureReceived", room.ID, message.TemperatureValue, room.TargetTemperature, message.MessageDate);

                    _logger.LogTrace($"Stored into DB: {message.TemperatureValue}° for {room.Name} at {message.MessageDate}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.ToString()}: {ex.Message}");
            }

        }
    }
}
