using Domotique.Model;
using Messages.Queue.Model;
using Messages.Queue.Service;
using Messages.WebMessages;
using System.Collections.Generic;

namespace Domotique.Service
{
    class DeviceService : IDeviceService
    {
        IQueueConnectionFactory _queueConnectionFactory;

        IDataRead _dataread;

        public DeviceService(IQueueConnectionFactory queueConnectionFactory, IDataRead dataread)
        {
            _queueConnectionFactory = queueConnectionFactory;
            _dataread = dataread;
        }

        public void PowerOff(long deviceID)
        {
            var publisher = _queueConnectionFactory.GetQueuePublisher<CommandMessage>("ZWaveCommand");
            Device device = _dataread.ReadDeviceByID(deviceID);
            var message = new CommandMessage()
            {
                Command = "PowerOff",
                MessageDate = System.DateTime.Now,
                TargetAdapter = "zwave",
                TargetAddress = device.Address
            };

            publisher.Publish(message);
        }


        public void PowerOn(long deviceID)
        {
            var publisher = _queueConnectionFactory.GetQueuePublisher<CommandMessage>("ZWaveCommand");
            Device device = _dataread.ReadDeviceByID(deviceID);
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
