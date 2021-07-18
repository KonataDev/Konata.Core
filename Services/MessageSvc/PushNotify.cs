using System;

using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushNotify", "Push notify on received a private message")]
    public class PushNotify : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
            => (output = new PrivateMessageNotifyEvent()) == output;

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}
