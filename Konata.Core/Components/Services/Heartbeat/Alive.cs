using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.Heartbeat;

[EventSubscribe(typeof(CheckHeartbeatEvent))]
[Service("Heartbeat.Alive", PacketType.TypeA, AuthFlag.DefaultlyNo, SequenceMode.Managed)]
internal class Alive : BaseService<CheckHeartbeatEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out CheckHeartbeatEvent output)
    {
        output = CheckHeartbeatEvent.Result(0);
        return true;
    }

    protected override bool Build(int sequence, CheckHeartbeatEvent input, 
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        return true;
    }
}
