using System;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Core.Attributes;

namespace Konata.Core.Service.Heartbeat
{
    [Service("Heartbeat.Alive", "Heartbeat for client")]
    public class Alive : IService
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
