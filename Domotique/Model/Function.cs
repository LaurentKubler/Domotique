using System;
using System.Collections.Generic;
using System.Text;

namespace Domotique.Model
{
    class Function
    {
        public int ID { get; set; }

        string Name { get; set; }

        int DeviceID { get; set; }

        Device Device { get; set; }
    }
}
