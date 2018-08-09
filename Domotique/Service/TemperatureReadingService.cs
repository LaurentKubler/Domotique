using Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void  SetStatusService(IStatusService service)
        {
            statusService = service;
        }

        

        public void Start()
        {           
            var factory = new ConnectionFactory() { HostName = ServerName };
            connection = factory.CreateConnection();
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

                                String roomName = "";

                                statusService.RegisterTemperature(roomName, temp.TemperatureValue,temp.MessageDate);

                                Console.WriteLine(" [x] Received {0}", message+":"+temp.ProbeAddress+"?"+temp.TemperatureValue);
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
