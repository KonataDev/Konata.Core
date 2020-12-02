using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.Events;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.OnlinePush
{
    [SSOService("OnlinePush.PbPushGroupMsg", "Push group message from server")]
    public class PbPushGroupMsg : ISSOService
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
