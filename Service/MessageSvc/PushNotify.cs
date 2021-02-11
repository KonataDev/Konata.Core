using System;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Core.Event.EventModel;

namespace Konata.Core.Service.MessageSvc
{
    [Service("MessageSvc.PushNotify", "Push notify on received a private message")]
    public class PushNotify : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
            => (output = new PrivateMessageNotifyEvent()) == output;

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}
