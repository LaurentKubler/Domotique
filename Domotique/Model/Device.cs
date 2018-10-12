using System.Collections.Generic;

namespace Domotique.Model
{
    public class Device
    {
        public string Name { get; set; }

        public string Adapter { get; set; }

        public string Address { get; set; }

        public string WriteAddress { get; set; }

        public string Value { get; set; }

        public IList<Function> Functions { get; set; }
    }
}
