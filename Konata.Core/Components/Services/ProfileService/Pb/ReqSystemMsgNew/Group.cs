using System.Collections.Generic;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf.Msf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.ProfileService.Pb.ReqSystemMsgNew;

[EventSubscribe(typeof(ReqSystemMsgGroupEvent))]
[Service("ProfileService.Pb.ReqSystemMsgNew.Group", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Group : BaseService<ReqSystemMsgGroupEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out ReqSystemMsgGroupEvent output, List<ProtocolEvent> extra)
    {
        // input.Payload
        var proto = structmsg.Types.RspSystemMsgNew
            .Parser.ParseFrom(input.Payload.GetBytes());
        {
            output = ReqSystemMsgGroupEvent.Create(proto.Head.Result);
            foreach (var msg in proto.Groupmsgs)
            {
                // Filter subtype 1
                if (msg.Msg.SubType != 1) continue;

                switch (msg.Msg.GroupMsgType)
                {
                    // Group invite
                    case 2:
                        extra.Add(GroupInviteEvent.Push((uint) msg.Msg.GroupCode,
                            msg.Msg.GroupName, (uint) msg.Msg.ActionUin, msg.Msg.ActionUinNick,
                            msg.Msg.GroupInviterRole > 1, (uint) msg.Time, (long) msg.Seq));
                        return true;

                    // Group request join
                    case 1:
                    case 22:
                        extra.Add(GroupRequestJoinEvent.Push((uint) msg.Msg.GroupCode,
                            msg.Msg.GroupName, (uint) msg.Msg.ActionUin, (uint) msg.ReqUin, msg.Msg.ReqUinNick,
                            msg.Msg.Additional, (uint) msg.Time, (long) msg.Seq
                        ));
                        return true;
                }
            }

            return true;
        }
    }

    protected override bool Build(int sequence, ReqSystemMsgGroupEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        var proto = new structmsg.Types.ReqSystemMsgNew
        {
            Num = 20,
            Version = 1000,
            Checktype = 3,
            Flag = new()
            {
                GrpMsgKickAdmin = 1,
                GrpMsgHiddenGrp = 1,
                GrpMsgWordingDown = 1,
                // FrdMsgGetBusiCard = 1,
                GrpMsgGetOfficialAccount = 1,
                GrpMsgGetPayInGroup = 1,
                // FrdMsgDiscuss2ManyChat = 1,
                GrpMsgNotAllowJoinGrpInviteNotFrd = 1,
                // FrdMsgNeedWaitingMsg = 1,
                // FrdMsgUint32NeedAllUnreadMsg = 1,
                GrpMsgNeedAutoAdminWording = 1,
                GrpMsgGetTransferGroupMsgFlag = 1,
                GrpMsgGetQuitPayGroupMsgFlag = 1,
                GrpMsgSupportInviteAutoJoin = 1,
                GrpMsgMaskInviteAutoJoin = 1,
                GrpMsgGetDisbandedByAdmin = 1,
                GrpMsgGetC2CInviteJoinGroup = 1
            },
            Language = 0,
            IsGetFrdRibbon = false,
            IsGetGrpRibbon = false,
            ReqMsgType = (uint) input.RequestMsgType,
        };

        output.PutProtoMessage(proto);
        return true;
    }
}
