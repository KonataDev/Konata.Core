using System;

using Konata.Core.Event;
using Konata.Core.Packet;

namespace Konata.Core.Service.MessageSvc
{
    [Service("MessageSvc.PushForceOffline", "Force offline")]
    public class PushForceOffline : IService
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
