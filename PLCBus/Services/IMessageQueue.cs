using Messages.Queue.Model;

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
