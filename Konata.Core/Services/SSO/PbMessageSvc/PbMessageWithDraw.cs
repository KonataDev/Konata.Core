using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.EventArgs;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.PbMessageSvc
{
    [SSOService("PbMessageSvc.PbMessageWithDraw", "Withdraw message")]
    public class PbMessageWithDraw : ISSOService
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
