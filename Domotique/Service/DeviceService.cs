﻿using Domotique.Database;
using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using Messages.WebMessages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domotique.Service
{
    class DeviceService : IDeviceService
    {
        IQueueConnectionFactory _queueConnectionFactory;

        IDataRead _dataread;

        DomotiqueContext _context;

        public DeviceService(IQueueConnectionFactory queueConnectionFactory, IDataRead dataread, DomotiqueContext context)
        {
            _queueConnectionFactory = queueConnectionFactory;
            _dataread = dataread;
            _context = context;
        }

        public void PowerOff(long deviceID)
        {
            //var publisher = _queueConnectionFactory.GetQueuePublisher<CommandMessage>("ZWaveCommand");
            var d = _context.Device.Where(c => c.DeviceID == deviceID).Include(dev => dev.Adapter).First();
            Console.Write("QueueTage :" + d.Adapter.QueueTag);
            var publisher = _queueConnectionFactory.GetQueuePublisher<CommandMessage>(d.Adapter.QueueTag);
            //Model.Device device = _dataread.ReadDeviceByID(deviceID);
            var message = new CommandMessage()
            {
                Command = "PowerOff",
                MessageDate = System.DateTime.Now,
                TargetAdapter = "zwave",
                TargetAddress = d.Address
            };

            publisher.Publish(message);
        }


        public void PowerOn(long deviceID)
        {
            var publisher = _queueConnectionFactory.GetQueuePublisher<CommandMessage>("ZWaveCommand");
            Model.Device device = _dataread.ReadDeviceByID(deviceID);
            var message = new CommandMessage()
            {
                Command = "PowerOn",
                MessageDate = System.DateTime.Now,
                TargetAdapter = "zwave",
                TargetAddress = device.Address
            };

            publisher.Publish(message);
        }

        public void PowerOn(long deviceID, long value)
        {

        }

        public List<DeviceStatus> GetDeviceStatus()
        {
            return null;
        }
    }
}
