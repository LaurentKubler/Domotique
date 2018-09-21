using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using System.IO.Ports;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace PLCBus.Services
{
    public class MessageQueue : IMessageQueue
    {
        readonly SerialPort port = new SerialPort("/dev/tty");

        CancellationToken _cancellationtocken;

        readonly string _messageFilter = string.Empty;

        readonly string _queueName;

        IConnection _connection;

        IModel _channel;

        string _commandExchange;

        string _responseExchange;

        string _responseTag;

        public event QueueMessageReceived OnMessage;


        public MessageQueue(string server, string commandExchange, string messageFilter, string responseExchange, string responseTag)
        {

            var factory = new ConnectionFactory() { HostName = server };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: commandExchange, type: "topic");
            _queueName = _channel.QueueDeclare().QueueName;
            _messageFilter = messageFilter;
            _commandExchange = commandExchange;
            _responseExchange = responseExchange;
            _responseTag = responseTag;
        }


        public void Connect()
        {
            //_cancellationtocken = cancellationtocken;
            _channel.QueueBind(queue: _queueName,
                                   exchange: _commandExchange,
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
                var command = new CommandMessage()
                {
                    Command="test",
                    TargetAdapter="tot"
                };
                OnMessage(command);
            };
            _channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);            

        }

        public void Publish(DeviceMessage message)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            _channel.ExchangeDeclare(exchange: _responseExchange,
                                    type: "topic");

            _channel.BasicPublish(exchange: _responseExchange,
                                 routingKey: _responseTag,
                                 basicProperties: null,
                                 body: body);
        }


        public void Disconnect()
        {

        }
    }
}
