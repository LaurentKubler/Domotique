using Domotique.Model;
using Domotique.Service.Log;
using Messages.Queue.Model;
using Messages.Queue.Service;
using System;
using System.Linq;

namespace Domotique.Service
{
    class DeviceStatusReadingService : IDeviceStatusReadingService
    {
        IQueueConnectionFactory _queueConnectionFactory;

        IQueueSubscriber<DeviceStatusMessage> _statusSubscriber;

        ILogService _logService;

        // IDataRead _dataRead;

        DBContextProvider _contextProvider;

        public DeviceStatusReadingService(IQueueConnectionFactory queueConnectionFactory, ILogService logService, IDataRead dataRead, DBContextProvider contextProvider)
        {
            _queueConnectionFactory = queueConnectionFactory;
            _logService = logService;
            //   _dataRead = dataRead;
            _contextProvider = contextProvider;
        }


        public bool IsStarted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetStatusService(IStatusService service)
        {

        }


        public void Start()
        {
            _statusSubscriber = _queueConnectionFactory.GetQueueSubscriber<DeviceStatusMessage>("DeviceStatus");
            _statusSubscriber.OnMessage += OnDeviceStatus;
            _statusSubscriber.Connect();
        }


        private void OnDeviceStatus(DeviceStatusMessage message)
        {
            try
            {
                using (var _context = _contextProvider.getContext())
                {
                    Console.WriteLine($"On Device status Received : {message.ToString()}");

                    int device_ID = _context.Device.Where(d => d.Address == message.DeviceAddress).First().DeviceID;// _dataRead.ReadDeviceIDByAddress(message.DeviceAddress, message.DeviceAdapter);
                    Console.WriteLine($"Device identified ad : {device_ID}");

                    if (string.Compare(message.Value, "false", true) == 0)
                        _logService.LogDeviceStatus(device_ID, 0, message.MessageDate);
                    else if (string.Compare(message.Value, "true", true) == 0)
                        _logService.LogDeviceStatus(device_ID, 100, message.MessageDate);
                    else
                        _logService.LogDeviceStatus(device_ID, int.Parse(message.Value), message.MessageDate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured in OnDeviceStatus: {ex.Message}:{ex.StackTrace}");
            }
        }
    }
}
