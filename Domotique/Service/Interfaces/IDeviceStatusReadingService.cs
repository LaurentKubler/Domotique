namespace Domotique.Service
{
    public interface IDeviceStatusReadingService
    {
        bool IsStarted { get; set; }

        void Start();
    }
}
