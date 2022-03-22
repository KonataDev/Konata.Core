using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;

// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace Konata.Core.Components.Services.SharpSvr;

[EventSubscribe(typeof(SharpSvrEvent))]
[Service("SharpSvr.c2sackMSF", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.EventBased)]
internal class C2sAckMsf : BaseService<SharpSvrEvent>
{
    protected override bool Build(int sequence, SharpSvrEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        if (input.Status == SharpSvrEvent.CallStatus.AckMsf)
        {
            output.PutBytes(input.AckPayload.GetBytes());
            return true;
        }
        return false;
    }
}
