using System.Threading.Tasks;

namespace Domotique.Service
{
    interface IQueueConnectionFactory
    {
        Task<IQueuePublisher<T>> GetQueuePublisher<T>(string queueTag);

        Task<IQueueSubscriber<T>> GetQueueSubScriber<T>(string queueTag);
    }
}
