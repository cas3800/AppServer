using System;
using System.Collections.Generic;

#nullable disable

namespace AppServer
{
    public partial class Player
    {
        public int PlayerId { get; set; }
        public bool? AllowDid { get; set; }
        public bool AllowPass { get; set; }
        public string Token { get; set; }
    }
}
