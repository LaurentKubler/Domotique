using Messages.Queue.Model;

namespace Domotique.Service
{
    public delegate void QueueMessageReceived<DeviceStatusMessage>(DeviceStatusMessage message);

    public interface IDeviceStatusReadingService
    {
        event QueueMessageReceived<DeviceStatusMessage> OnMessage;

        bool IsStarted { get; set; }

        void Start();
    }
}
