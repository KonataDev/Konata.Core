using System;

using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.Heartbeat
{
    [SSOService("Heartbeat.Alive", "Heartbeat for client")]
    public class Alive : ISSOService
    {
        public bool DeSerialize(KonataEventArgs original, out KonataEventArgs evnentpackage)
        {
            throw new NotImplementedException();
        }

        public bool Serialize(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
