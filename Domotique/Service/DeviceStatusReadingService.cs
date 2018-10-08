﻿using Domotique.Model;
using Domotique.Service.Log;
using Messages.Queue.Model;
using Messages.Queue.Service;
using System;

namespace Domotique.Service
{
    class DeviceStatusReadingService : IDeviceStatusReadingService
    {
        IQueueConnectionFactory _queueConnectionFactory;

        IQueueSubscriber<DeviceStatusMessage> _statusSubscriber;

        ILogService _logService;

        IDataRead _dataRead;


        public DeviceStatusReadingService(IQueueConnectionFactory queueConnectionFactory, ILogService logService, IDataRead dataRead)
        {
            _queueConnectionFactory = queueConnectionFactory;
            _logService = logService;
            _dataRead = dataRead;
        }


        public bool IsStarted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetStatusService(IStatusService service)
        {

        }


        public void Start()
        {
            _statusSubscriber = _queueConnectionFactory.GetQueueSubScriber<DeviceStatusMessage>("DeviceStatus");
            _statusSubscriber.OnMessage += OnDeviceStatus;
            _statusSubscriber.Connect();
        }


        private void OnDeviceStatus(DeviceStatusMessage message)
        {
            Console.WriteLine(message.ToString());

            int device_ID = _dataRead.ReadDeviceIDByAddress(message.DeviceAdapter, message.DeviceAdapter);

            _logService.LogDeviceStatus(device_ID, message.Value, message.MessageDate);
        }
    }
}