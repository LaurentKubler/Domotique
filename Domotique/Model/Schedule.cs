using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model
{
    class Schedule
    {
        Period Period { get; set; }

        int Priority { get; set; }

        int TargetTemperature { get; set; }

    }
}
