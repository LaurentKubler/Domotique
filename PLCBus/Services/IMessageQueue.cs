using Messages.Queue.Model;

namespace PLCBus.Services
{
    public interface IMessageQueue
    {
        void Connect();

        void Publish(DeviceStatusMessage message);

        event QueueMessageReceived OnMessage;

        void Disconnect();
    }
}
