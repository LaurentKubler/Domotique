namespace Messages.Queue.Service
{
    public interface IQueueConnectionFactory
    {
        IQueuePublisher<T> GetQueuePublisher<T>(string queueTag);

        IQueueSubscriber<T> GetQueueSubScriber<T>(string queueTag);
    }
}
