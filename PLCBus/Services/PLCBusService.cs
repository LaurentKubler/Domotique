using Messages.Queue.Model;
using System;

namespace PLCBus.Services
{
    public delegate void QueueMessageReceived(CommandMessage message);

    public delegate void PLCBusMessageReceived();


    public class PLCBusService : IPLCBusService
    {
        readonly IMessageQueue _messageQueue;

        public PLCBusService(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
            messageQueue.Connect();

            messageQueue.OnMessage += OnMQMessage;

        }

        private void OnMQMessage(CommandMessage command)
        {
            Console.WriteLine("Message received");
        }
    }
}
