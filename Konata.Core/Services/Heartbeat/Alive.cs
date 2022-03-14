using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.Heartbeat;

[EventSubscribe(typeof(CheckHeartbeatEvent))]
[Service("Heartbeat.Alive", "Heartbeat for client")]
internal class Alive : IService
{
    public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
    {
        output = CheckHeartbeatEvent.Result(0);
        return true;
    }

    public bool Build(Sequence sequence, ProtocolEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        if (SSOFrame.Create("Heartbeat.Alive", PacketType.TypeA,
                newSequence, sequence.Session, new ByteBuffer(), out var ssoFrame))
        {
            if (ServiceMessage.Create(ssoFrame, AuthFlag.DefaultlyNo,
                    0x00, null, null, out var toService))
            {
                return ServiceMessage.Build(toService, device, out output);
            }
        }

        return false;
    }
}
