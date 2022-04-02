using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf.Msf;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Components.Services.ProfileService.Pb.ReqSystemMsgAction;

[EventSubscribe(typeof(GroupInviteEvent))]
[EventSubscribe(typeof(GroupRequestJoinEvent))]
[Service("ProfileService.Pb.ReqSystemMsgAction.Group", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Group : BaseService<ProtocolEvent>
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
        switch (input)
        {
            case GroupInviteEvent gie:
                output.PutProtoMessage(new structmsg.Types.ReqSystemMsgAction
                {
                    Type = 1,
                    Seq = (ulong) gie.Token,
                    ReqUin = gie.InviterUin,
                    SubType = 1,
                    SrcId = 3,
                    SubSrcId = 10016,
                    GroupMsgType = 2,
                    ActionInfo = new()
                    {
                        Type = gie.IsApproved ? 11U : 12U,
                        GroupCode = gie.GroupUin,
                        Blacklist = !gie.IsApproved && gie.PreventRequest
                    }
                });
                return true;

            case GroupRequestJoinEvent grj:
                output.PutProtoMessage(new structmsg.Types.ReqSystemMsgAction
                {
                    Type = 1,
                    Seq = (ulong) grj.Token,
                    ReqUin = grj.ReqUin,
                    SubType = 1,
                    SrcId = 3,
                    SubSrcId = 31,
                    GroupMsgType = 1,
                    ActionInfo = new()
                    {
                        Type = grj.IsApproved ? 11U : 12U,
                        GroupCode = grj.GroupUin,
                        Blacklist = !grj.IsApproved && grj.PreventRequest
                    }
                });
                return true;
        }

        return false;
    }
}
