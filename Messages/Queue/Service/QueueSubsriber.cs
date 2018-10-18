using Domotique.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Queue.Service
{
    class QueueSubscriber<T> : IQueueSubscriber<T>
    {
        public event QueueMessageReceived<T> OnMessage;

        private bool _quit = false;

        QueueConfiguration Configuration;

        private IModel _channel;

        public QueueSubscriber(QueueConfiguration configuration)
        {
            Configuration = configuration;
        }


        async public Task Connect()
        {
            var factory = new ConnectionFactory() { HostName = Configuration.ServerName };

            _channel = factory.CreateConnection().CreateModel();
            _channel.ExchangeDeclare(exchange: Configuration.Exchange, type: "topic");

            var _queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queue: _queueName, exchange: Configuration.Exchange, routingKey: Configuration.DefaultRoutingKey);
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: CreateConsumer(_channel));

            while (!_quit)
                await Task.Delay(1000);
        }


        private EventingBasicConsumer CreateConsumer(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    var command = JsonConvert.DeserializeObject<T>(message);

                    OnMessage(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occured reading message from queue: original message is {ea.Body} for routing key '{ea.RoutingKey}'");
                }
            };

            return consumer;
        }


        public void Disconnect()
        {
            _quit = true;
        }
    }
}
