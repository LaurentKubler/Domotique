namespace Domotique.Service
{
    interface IQueuePublisher<T>
    {
        void Publish(T message);
    }
}
