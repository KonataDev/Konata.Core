using System.Collections.Generic;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf.Msf;

// ReSharper disable UnusedType.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Components.Services.ProfileService.Pb.ReqSystemMsgNew;

[EventSubscribe(typeof(ReqSystemMsgFriendEvent))]
[Service("ProfileService.Pb.ReqSystemMsgNew.Friend", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Friend : BaseService<ReqSystemMsgFriendEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out ReqSystemMsgFriendEvent output, List<ProtocolEvent> extra)
    {
        // input.Payload
        var proto = structmsg.Types.RspSystemMsgNew
            .Parser.ParseFrom(input.Payload.GetBytes());
        {
            output = ReqSystemMsgFriendEvent.Create(proto.Head.Result);
            foreach (var msg in proto.Friendmsgs)
            {
                FilterableEvent outEvent = null;
                switch (msg.Msg.SubType)
                {
                    // Friend requests
                    case 1:
                        outEvent = FriendRequestEvent.Push((uint) msg.ReqUin,
                            msg.Msg.ReqUinNick, (uint) msg.Time, msg.Msg.Additional, (long) msg.Seq
                        );
                        break;

                    // Friend added (single)
                    // case 9:
                    //     extra.Add(FriendAddEvent.Push((uint) msg.Msg.GroupCode,
                    //         msg.Msg.GroupName, (uint) msg.Msg.ActionUin, (uint) msg.ReqUin, msg.Msg.ReqUinNick,
                    //         msg.Msg.Additional, (uint) msg.Time, (long) msg.Seq
                    //     ));
                    //     return true;
                }

                // Append events
                if (outEvent != null)
                {
                    extra.Add(outEvent);
                    outEvent.SetFilterIdenfidentor((uint) msg.Time, (uint) msg.Seq);
                }
            }

            return true;
        }
    }

    protected override bool Build(int sequence, ReqSystemMsgFriendEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        var proto = new structmsg.Types.ReqSystemMsgNew
        {
            Num = (uint) input.RequestNum,
            Version = 1000,
            Checktype = 2,
            Flag = new()
            {
                FrdMsgGetBusiCard = 1,
                FrdMsgDiscuss2ManyChat = 1,
                FrdMsgNeedWaitingMsg = 1,
                FrdMsgUint32NeedAllUnreadMsg = 1,
            },
            Language = 0,
            IsGetFrdRibbon = false,
            IsGetGrpRibbon = false,
            FriendMsgTypeFlag = 1,
            ReqMsgType = 2,
        };

        output.PutProtoMessage(proto);
        return true;
    }
}
