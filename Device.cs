using System;
using System.Collections.Generic;

#nullable disable

namespace AppServer
{
    public partial class Device
    {
        public int PlayerId { get; set; }
        public string DeviceId { get; set; }

        public virtual Player Player { get; set; }
    }
}
