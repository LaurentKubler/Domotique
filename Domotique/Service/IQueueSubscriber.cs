namespace Domotique.Service
{
    public delegate void QueueMessageReceived<T>(T message);

    interface IQueueSubscriber<T>
    {
        event QueueMessageReceived<T> OnMessage;

        void Connect();

    }
}
