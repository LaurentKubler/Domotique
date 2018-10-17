using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace Domotique.Service
{
    public class TemperatureReadingService : IDisposable, ITemperatureReadingService
    {

        public static String ServerName { get; set; }

        public static String QueueName { get; set; }

        public bool IsStarted { get; set; } = false;

        private IConnection connection;

        private IModel channel;

        private IStatusService statusService;

        private IDataRead _dataRead;

        private IQueueConnectionFactory _queueConnectionFactory;

        private IQueueSubscriber<ProbeTemperatureMessage> subscriber;

        //private DbContextOptions<DomotiqueContext> _options;
        private DBContextProvider _provider;

        public TemperatureReadingService(IDataRead dataRead, IQueueConnectionFactory queueConnectionFactory, DBContextProvider provider)
        {
            _dataRead = dataRead;
            _provider = provider;
            _queueConnectionFactory = queueConnectionFactory;
        }


        public void SetStatusService(IStatusService service)
        {
            statusService = service;
        }


        public void Start()
        {
            subscriber = _queueConnectionFactory.GetQueueSubscriber<ProbeTemperatureMessage>("Temperature");
            subscriber.OnMessage += Received;
            subscriber.Connect();
            /*var factory = new ConnectionFactory() { HostName = ServerName };
            while (connection == null)
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (Exception e)
                {
                    Console.Write("unable to reach rabbitmq: " + e.Message);
                    Thread.Sleep(10000);
                }

            }
            channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;

                    var message = Encoding.UTF8.GetString(body);
                    var temp = JsonConvert.DeserializeObject<ProbeTemperatureMessage>(message);

                    string roomName = _dataRead.ReadRoomNameByProbe(temp.ProbeAddress);

                    statusService.RegisterTemperature(roomName, temp.TemperatureValue, temp.MessageDate);
                };

                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            }

            IsStarted = true;*/
        }

        public void Dispose()
        {
            subscriber.Disconnect();
        }


        private void Received(ProbeTemperatureMessage message)
        {
            using (var dbContext = _provider.getContext())// new DomotiqueContext(_options))
            {
                var room = dbContext.Rooms.Where(c => c.Captor.StatusAddress == message.ProbeAddress).First();
                statusService.RegisterTemperature(room.Name, message.TemperatureValue, message.MessageDate);
            }
            //string roomName = _dataRead.ReadRoomNameByProbe(message.ProbeAddress);


        }
    }
}
