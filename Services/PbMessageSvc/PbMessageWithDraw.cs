using System;

using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;

namespace Konata.Core.Services.PbMessageSvc
{
    [Service("PbMessageSvc.PbMessageWithDraw", "Withdraw message")]
    public class PbMessageWithDraw : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            throw new NotImplementedException();
        }
    }
}
