namespace Messages.Queue.Service
{
    public interface IQueuePublisher<T>
    {
        void Publish(T message);
    }
}
