using System;

using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Attributes;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushReaded", "Push have been read this message")]
    public class PushRead : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
            => (output = null) == null;

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => throw new NotImplementedException();
    }
}
