using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.PbMessageSvc;

[EventSubscribe(typeof(FriendMessageRecallEvent))]
[EventSubscribe(typeof(GroupMessageRecallEvent))]
[Service("PbMessageSvc.PbMsgWithDraw", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbMsgWithDraw : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo, BotKeyStore keystore,
        out ProtocolEvent output)
    {
        var pb = ProtobufDecoder.Create(input.Payload);
        output = new ProtocolEvent((int) pb[2][1].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, ProtocolEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        switch (input)
        {
            case GroupMessageRecallEvent e:
                output.PutProtoNode(new ProtoMsgWithDraw(e.GroupUin, e.Sequence, e.Random));
                break;
            case FriendMessageRecallEvent e:
                output.PutProtoNode(new ProtoMsgWithDraw(keystore.Account.Uin, e.FriendUin, e.Sequence, e.Random, e.Uuid, e.Time));
                break;
        }
        return true;
    }
}
