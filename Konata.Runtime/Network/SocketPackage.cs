using Konata.Runtime.Base.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Runtime.Network
{
    public class SocketPackage:KonataEventArgs
    {
        public byte[] Data { get; set; }
    }
}
