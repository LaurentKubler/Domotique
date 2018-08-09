using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Domotique
{
    class StatusMessage
    {
        public StatusMessage()
        {
            Time = DateTime.Now;           
        }

        public int DeviceId { get; }

        public DateTime Time { get; private set; }

        public String Type { get; set; }

        public String Data { get; set; }

    }

    class Send
    {
        public static void Msain()
        {

            string queueName = "tesdt";
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var bdy = ea.Body;
                    var messages = Encoding.UTF8.GetString(bdy);
                    var readMessage = JsonConvert.DeserializeObject<StatusMessage>(messages);
                    Console.WriteLine(" [x] Received {0}", readMessage.Time);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                var status = new StatusMessage()
                {
                    Data = "25.2",
                    Type = "Temperature"
                };
                string message =JsonConvert.SerializeObject(status);                
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                status = new StatusMessage()
                {                    
                    Data = "25s",
                    Type = "Temsperature"
                };
                message = JsonConvert.SerializeObject(status);
                body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                status = new StatusMessage()
                {
                    Data = "25s",
                    Type = "Temsperature"
                };
                message = JsonConvert.SerializeObject(status);
                body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                status = new StatusMessage()
                {
                    Data = "25s",
                    Type = "Temsperature"
                };
                message = JsonConvert.SerializeObject(status);
                body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                Console.ReadLine();
            }

            Console.WriteLine(" Press [enter] to exit.");
            
        }
    }

    class Receive
    {
        public static void Mains()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
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
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
