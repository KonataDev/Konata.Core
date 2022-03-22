using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcPush;
using Konata.Core.Utils.Protobuf;

// ReSharper disable InconsistentNaming

namespace Konata.Core.Components.Services.SharpSvr;

[EventSubscribe(typeof(SharpSvrEvent))]
[Service("SharpSvr.c2sack", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.EventBased)]
internal class C2sAck : BaseService<SharpSvrEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out SharpSvrEvent output)
    {
        var push = new SvcPushSharpVideoMsg(input.Payload.GetBytes());
        var pbtree = ProtobufDecoder.Create(push.PbBody);
        output = SharpSvrEvent.PushAck(push.Status, pbtree[1][1].AsNumber(), (uint) pbtree[3][4].AsNumber());
        return true;
    }
}



