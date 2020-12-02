using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.StatSvc
{
    [SSOService("StatSvc.Register", "Register client")]
    public class Register : ISSOService
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