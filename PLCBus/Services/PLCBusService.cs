using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLCBus.Services
{
    public delegate void QueueMessageReceived(CommandMessage message);

    public delegate void PLCBusMessageReceived();


    public class PLCBusService : IPLCBusService
    {
        IMessageQueue _messageQueue;

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
