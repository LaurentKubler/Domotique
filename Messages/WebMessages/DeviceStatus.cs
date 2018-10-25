using System;
using System.Collections.Generic;

namespace Messages.WebMessages
{
    public class DeviceStatus
    {
        public long Device_ID { get; set; }

        public string DeviceName { get; set; }

        public long? Value { get; set; }

        public bool? Status { get; set; }

        public int OnImage_ID { get; set; }

        public int OffImage_ID { get; set; }

        public DateTime? ValueDate { get; set; }

        public IList<Function> Functions { get; set; } = new List<Function>();

        public object ToList()
        {
            throw new NotImplementedException();
        }
    }
}
