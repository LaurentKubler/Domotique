using Domotique.Model;
using Domotique.Service.Log;
using Messages.Queue.Model;
using Messages.Queue.Service;
using Microsoft.Extensions.Logging;
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

        IDBContextProvider _contextProvider;

        ILogger<DeviceStatusReadingService> _logger;

        public DeviceStatusReadingService(IQueueConnectionFactory queueConnectionFactory, ILogService logService, IDataRead dataRead, IDBContextProvider contextProvider, ILogger<DeviceStatusReadingService> logger)
        {
            _queueConnectionFactory = queueConnectionFactory;
            _logService = logService;
            _logger = logger;
            _contextProvider = contextProvider;
        }


        public bool IsStarted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


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
                    _logger.LogTrace($"On Device status Received : {message.ToString()}");

                    int device_ID = _context.Device.Where(d => d.Address == message.DeviceAddress).First().DeviceID;// _dataRead.ReadDeviceIDByAddress(message.DeviceAddress, message.DeviceAdapter);
                    _logger.LogTrace($"Device identified ad : {device_ID}");

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
                _logger.LogError($"Exception occured in OnDeviceStatus: {ex.Message}:{ex.StackTrace}");
            }
        }
    }
}
