using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    public class TemperatureReadingService
    {
        public String ServerName { get; set; }

        public String QueueName { get; set; }

        public int ServerPort { get; set; }

        public bool IsStarted { get; set; } = false;

        public void Start()
        {           
            var factory = new ConnectionFactory() { HostName = ServerName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
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
                                Console.WriteLine(" [x] Received {0}", message);
                            };
                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            }
            IsStarted = true;
        }
    }
}
