using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.SharpSvr;

[EventSubscribe(typeof(SharpSvrEvent))]
[Service("SharpSvr.c2s", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.EventBased)]
internal class C2s : BaseService<SharpSvrEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out SharpSvrEvent output)
    {
        output = SharpSvrEvent.Result(0);
        return true;
    }

    protected override bool Build(int sequence, SharpSvrEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        switch (input.Status)
        {
            case SharpSvrEvent.CallStatus.CallOut:
                var sharpMsg = new SharpVideoMsg(input.SelfUin, input.CallUin);
                output = new SvcReqSharpVideoMsg(input.SelfUin, input.CallUin, sharpMsg);
                return true;

            case SharpSvrEvent.CallStatus.Ack:
                var ackMsg = new SharpVideoMsg.Ack(input.RoomId, input.SelfUin);
                output = new SvcReqSharpVideoMsg(input.SelfUin, input.CallUin, ackMsg);
                return true;

            default:
                return false;
        }
    }
}
