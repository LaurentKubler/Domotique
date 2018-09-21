using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLCBus.Services
{
    public interface IMessageQueue
    {
        void Connect();

        void Publish(DeviceMessage message);

        event QueueMessageReceived OnMessage;

        void Disconnect();
    }
}
