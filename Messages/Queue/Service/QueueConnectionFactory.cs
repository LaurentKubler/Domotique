using Domotique.Model;
using System.Collections.Generic;

namespace Messages.Queue.Service
{
    public class QueueConnectionFactory : IQueueConnectionFactory
    {
        private List<QueueConfiguration> _queues;
        public QueueConnectionFactory(List<QueueConfiguration> queues)
        {
            _queues = queues;
        }


        public IQueuePublisher<T> GetQueuePublisher<T>(string queueTag)
        {
            foreach (var queue in _queues)
            {
                if (queueTag == queue.QueueApplicationTag)
                    return new QueuePublisher<T>(queue);
            }
            throw new System.Exception("Queue not defined{queueTag}");
        }


        public IQueueSubscriber<T> GetQueueSubScriber<T>(string queueTag)
        {
            foreach (var queue in _queues)
            {
                if (queueTag == queue.QueueApplicationTag)
                    return new QueueSubscriber<T>(queue);
            }
            throw new System.Exception("Queue not defined{queueTag}");
        }
    }
}
