using System.Threading.Tasks;

namespace Messages.Queue.Service
{
    public delegate void QueueMessageReceived<T>(T message);

    public interface IQueueSubscriber<T>
    {
        event QueueMessageReceived<T> OnMessage;

        Task Connect();

        void Disconnect();
    }
}
