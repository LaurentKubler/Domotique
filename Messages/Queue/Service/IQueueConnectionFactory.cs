using System.Threading.Tasks;

namespace Messages.Queue.Service
{
    public interface IQueueConnectionFactory
    {
        Task<IQueuePublisher<T>> GetQueuePublisher<T>(string queueTag);

        Task<IQueueSubscriber<T>> GetQueueSubScriber<T>(string queueTag);
    }
}
