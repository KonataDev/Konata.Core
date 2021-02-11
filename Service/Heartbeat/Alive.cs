using System;

using Konata.Core.Event;
using Konata.Core.Packet;

namespace Konata.Core.Service.Heartbeat
{
    [Service("Heartbeat.Alive", "Heartbeat for client")]
    public class Alive : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            throw new NotImplementedException();
        }
    }
}
