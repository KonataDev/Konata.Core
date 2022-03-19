using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Message;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[EventSubscribe(typeof(GroupMessageEvent))]
[Service("MessageSvc.PbSendMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbSendMsg : BaseService<GroupMessageEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupMessageEvent output)
    {
        var pb = ProtobufDecoder.Create(input.Payload);
        output = GroupMessageEvent.Result((int) pb[1].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, GroupMessageEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new GroupMsg(input.GroupUin, MessagePacker.PackUp(input.Message.Chain)));
        return true;
    }
}
