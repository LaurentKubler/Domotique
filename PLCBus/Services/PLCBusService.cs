using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLCBus.Services
{
    public class PLCBusService : IPLCBusService
    {
        IMessageQueue _messageQueue;


        public PLCBusService(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
            messageQueue.Connect();
            

        }
    }
}
