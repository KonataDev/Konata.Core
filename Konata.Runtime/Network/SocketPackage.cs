using System;

using Konata.Runtime.Base.Event;

namespace Konata.Runtime.Network
{
    public class SocketPackage : KonataEventArgs
    {
        public byte[] Data { get; set; }
    }
}
