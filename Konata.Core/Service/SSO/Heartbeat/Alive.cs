using System;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.Heartbeat
{
    [SSOService("Heartbeat.Alive", "Heartbeat for client")]
    public class Alive : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoMessage, out KonataEventArgs output)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutGoing(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
