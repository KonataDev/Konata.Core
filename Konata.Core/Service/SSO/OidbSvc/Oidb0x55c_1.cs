using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.OidbSvc
{
    [SSOService("OidbSvc.0x55c_1", "Promote admin for member")]
    public class Oidb0x55c_1 : ISSOService
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
