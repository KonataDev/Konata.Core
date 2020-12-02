using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.Events;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.MessageSvc
{
    [SSOService("MessageSvc.PushReaded", "Push have been read this message")]
    public class PushRead : ISSOService
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
