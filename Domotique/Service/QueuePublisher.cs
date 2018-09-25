using Domotique.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Service
{
    class QueuePublisher<T> : IQueuePublisher<T>
    {
        private QueueConfiguration Configuration { set; get; }

        private List<T> _unpublishedMessages = new List<T>();

        public QueuePublisher(QueueConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Publish(T message)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = Configuration.ServerName, Port = Configuration.ServerPort };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Configuration.Exchange, type: "fanout");
                    var stringMessage = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(stringMessage);

                    channel.BasicPublish(exchange: Configuration.Exchange,
                                         routingKey: Configuration.DefaultRoutingKey,
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch (Exception e)
            {
                _unpublishedMessages.Add(message);
                Console.WriteLine(e);
            }
        }
    }
}
