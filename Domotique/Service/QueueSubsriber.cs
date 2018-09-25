using Domotique.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Domotique.Service
{
    class QueueSubscriber<T> : IQueueSubscriber<T>
    {
        public event QueueMessageReceived<T> OnMessage;

        QueueConfiguration Configuration;


        public QueueSubscriber(QueueConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Connect()
        {
            var factory = new ConnectionFactory() { HostName = Configuration.ServerName };
            var _connection = factory.CreateConnection();
            IModel _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: Configuration.Exchange, type: "topic");
            var _queueName = _channel.QueueDeclare().QueueName;
            var _messageFilter = Configuration.DefaultRoutingKey;

            _channel.QueueBind(queue: _queueName,
                                  exchange: Configuration.Exchange,
                                  routingKey: _messageFilter);
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey,
                                  message);
                var command = JsonConvert.DeserializeObject<T>(message);
                OnMessage(command);
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
