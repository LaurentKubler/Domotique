using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLCBus.Model
{
    public class PLCCommand
    {
        enum ECommandType { AllUnitOff, AllLightsOn, On, Off, Dim, Bright, AllLightOff, AllUserLightsOn, AllUserLightsOff, StatusOn, StatusOff, GetAllID
        };

        PLCBusAddress Address { get; set; }

        ECommandType Command { get; set; }

        int Value { get; set; }


        PLCCommand(CommandMessage message)
        {
            switch (message.Command)
            {

            }
            Address = new PLCBusAddress(message.TargetAddress);
        }
    }
}
