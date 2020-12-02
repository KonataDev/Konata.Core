using System;

using Konata.Core.Packet;
using Konata.Core.Events;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.Heartbeat
{
    [SSOService("Heartbeat.Alive", "Heartbeat for client")]
    public class Alive : ISSOService
    {
        public bool HandleInComing(SSOMessage ssoMessage, out KonataEventArgs output)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutGoing(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
