using System;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Core.Attributes;

namespace Konata.Core.Service.MessageSvc
{
    [Service("MessageSvc.PushReaded", "Push have been read this message")]
    public class PushRead : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
            => (output = null) == null;

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
            => throw new NotImplementedException();
    }
}
