using System;
using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;

namespace Konata.Core.Services.MessageSvc;

[Service("MessageSvc.PbDeleteMsg", "Delete message")]
internal class PbDeleteMsg : IService
{
    public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
    {
        throw new NotImplementedException();
    }

    public bool Build(Sequence sequence, ProtocolEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        throw new NotImplementedException();
    }
}
