using Domotique.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Messages.Queue.Service
{
    public class QueueConnectionFactory : IQueueConnectionFactory
    {
        public QueueConnectionFactory(List<QueueConfiguration> queues)
        {

        }


        public Task<IQueuePublisher<T>> GetQueuePublisher<T>(string queueTag)
        {
            throw new System.NotImplementedException();
        }


        public Task<IQueueSubscriber<T>> GetQueueSubScriber<T>(string queueTag)
        {
            throw new System.NotImplementedException();
        }
    }
}
