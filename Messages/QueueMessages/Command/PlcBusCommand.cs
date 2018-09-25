using System;
using System.Collections.Generic;
using System.Text;

namespace Messages.QueueMessages.Command
{
    class PlcBusCommand
    {
        string HomeUnit { get; set; }

        string UserCode { get; set; }

        string Command { get; set; }

        long CommandDate { get; set; }
    }
}
