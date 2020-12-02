using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.Events;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.OidbSvc
{
    [SSOService("OidbSvc.0x8a0_0", "Kick members in the group")]
    class Oidb0x8a0_0 : ISSOService
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
