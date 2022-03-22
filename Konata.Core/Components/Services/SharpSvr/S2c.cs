using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;

// ReSharper disable InconsistentNaming

namespace Konata.Core.Components.Services.SharpSvr;

[EventSubscribe(typeof(SharpSvrEvent))]
[Service("SharpSvr.s2c", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.EventBased)]
internal class S2c : BaseService<SharpSvrEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out SharpSvrEvent output)
    {
        output = SharpSvrEvent.PushAckMsf(input.Payload);
        return true;
    }
}
