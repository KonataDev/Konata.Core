using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Message;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[EventSubscribe(typeof(GroupMessageEvent))]
[EventSubscribe(typeof(FriendMessageEvent))]
[Service("MessageSvc.PbSendMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbSendMsg : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ProtocolEvent output)
    {
        var pb = ProtobufDecoder.Create(input.Payload);
        output = new ProtocolEvent((int) pb[1].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, ProtocolEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        switch (input)
        {
            case GroupMessageEvent gme:
                output.PutProtoNode(new GroupMsg(gme.GroupUin, 
                    MessagePacker.PackUp(gme.Message.Chain, MessagePacker.Mode.Group)));
                break;

            case FriendMessageEvent fme:
                output.PutProtoNode(new FriendMsg(fme.FriendUin,
                    keystore.Account.SyncCookieConsts, MessagePacker.PackUp(fme.Chain, MessagePacker.Mode.Friend)));
                break;
        }

        return true;
    }
}
