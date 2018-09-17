using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO.Ports;
namespace PLCBus.Services
{
    public class MessageQueueReceiver : IMessageQueueReceiver
    {
        readonly SerialPort port = new SerialPort("/dev/tty");
    }
}
