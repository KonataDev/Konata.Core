using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf.Msf;

namespace Konata.Core.Components.Services.ProfileService.Pb.ReqSystemMsgAction;

[EventSubscribe(typeof(FriendRequestEvent))]
[Service("ProfileService.Pb.ReqSystemMsgAction.Friend", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Friend : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out ProtocolEvent output)
    {
        var proto = structmsg.Types
            .RspSystemMsgAction.Parser.ParseFrom(input.Payload.GetBytes());
        {
            output = new ProtocolEvent(proto.Head.Result);
            return true;
        }
    }
    
    protected override bool Build(int sequence, ProtocolEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        var fre = (FriendRequestEvent) input;
        output.PutProtoMessage(new structmsg.Types.ReqSystemMsgAction
        {
            Type = 1,
            Seq = (ulong) fre.Token,
            ReqUin = fre.ReqUin,
            SubType = 1,
            SrcId = 6,
            SubSrcId = 7,
            ActionInfo = new()
            {
                Type = fre.IsApproved ? 2U : 3U,
                Blacklist = !fre.IsApproved && fre.PreventRequest
            }
        });
        return true;
    }
}
