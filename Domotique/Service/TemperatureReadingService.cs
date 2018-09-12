using Domotique.Model;
using Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Domotique.Service
{
    public class TemperatureReadingService : IDisposable, ITemperatureReadingService
    {

        public String ServerName { get; set; }

        public String QueueName { get; set; }

        public int ServerPort { get; set; }

        public bool IsStarted { get; set; } = false;

        private IConnection connection;

        private IModel channel;

        private IStatusService statusService;

        private IDataRead _dataRead;

        public TemperatureReadingService(IDataRead dataRead)
        {
            _dataRead = dataRead;
        }
        public void SetStatusService(IStatusService service)
        {
            statusService = service;
        }


        public void Start()
        {

            var factory = new ConnectionFactory() { HostName = ServerName };
            while (connection == null)
            {
                try
                {
                    connection = factory.CreateConnection();
                }
                catch (Exception e)
                {
                    Console.Write("unable to reach rabbitmq");
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

                                String roomName = _dataRead.ReadRoomNameByProbe(temp.ProbeAddress);

                                statusService.RegisterTemperature(roomName, temp.TemperatureValue, temp.MessageDate);
                            };
                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            }

            IsStarted = true;
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
    }
}
