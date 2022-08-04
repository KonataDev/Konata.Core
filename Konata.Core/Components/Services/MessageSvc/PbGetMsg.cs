using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf.ProtoModel;
using Konata.Core.Utils;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[EventSubscribe(typeof(PbGetMessageEvent))]
[Service("MessageSvc.PbGetMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbGetMsg : BaseService<PbGetMessageEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out PbGetMessageEvent output, List<ProtocolEvent> extra)
    {
        var root = ProtoTreeRoot.Deserialize(input.Payload, true);

        // Get sync cookie 
        root.TryGetLeafBytes("1A", out var cookie);

        var root2A = root.GetLeaves<ProtoTreeRoot>("2A");
        if (root2A == null) goto Finish;

        foreach (var i in root2A)
        {
            var root22 = i.GetLeaves<ProtoTreeRoot>("22");
            if (root22 == null) continue;

            foreach (var j in root22)
            {
                j.GetTree("0A", _ =>
                {
                    try
                    {
                        var type = (NotifyType) _.GetLeafVar("18");
                        switch (type)
                        {
                            case NotifyType.FriendMessage:
                            case NotifyType.FriendMessageSingle:
                            case NotifyType.FriendPttMessage:
                            case NotifyType.FriendFileMessage:
                                extra.Add(OnProcessMessage(keystore.Account.Uin, j));
                                break;

                            case NotifyType.NewMember:
                                extra.Add(OnProcessNewMember(keystore.Account.Uin, j));
                                break;

                            case NotifyType.StrangerMessage:
                                break;
                            
                            default:
                            case NotifyType.GroupCreated:
                            case NotifyType.GroupRequestAccepted:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        // TODO: Droppppppp
                        // Fixme
                    }
                });
            }
        }

        Finish:
        output = PbGetMessageEvent.Result(0, cookie);
        return true;
    }

    protected override bool Build(int sequence, PbGetMessageEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new GetMessageRequest(input.SyncCookie));
        return true;
    }

    private FriendMessageEvent OnProcessMessage(uint selfUin, ProtoTreeRoot root)
    {
        var pb = root.PathTo<ProtoTreeRoot>("0A");

        var message = FriendMessageEvent.Push();
        var context = new MessageStruct(MessageStruct.SourceType.Friend);
        message.SetMessageStruct(context);
        {
            var toUin = (uint) pb.GetLeafVar("10");
            var fromUin = (uint) pb.GetLeafVar("08");

            if (fromUin == selfUin && toUin != selfUin)
                throw new Exception("skip self-sent message");

            var sequence = (uint) pb.GetLeafVar("28");
            var time = (uint) pb.GetLeafVar("30");
            var uuid = pb.GetLeafVar("38");

            var rand = !root.TryPathTo<ProtoVarInt>("1A.0A.0A.18", out var x)
                ? (uint) (uuid & 0xFFFFFFFF)
                : x;

            var chains = MessagePacker.UnPack(root.PathTo<ProtoTreeRoot>("1A"), MessagePacker.Mode.Friend);

            context.SetMessage(chains);
            context.SetSenderInfo(fromUin, "");
            context.SetReceiverInfo(toUin, "");
            context.SetSourceInfo(sequence, rand, time, uuid);
            message.SetFilterIdenfidentor(time, rand);
        }

        message.SetSelfUin(selfUin);
        return message;
    }

    private GroupMemberIncreaseEvent OnProcessNewMember(uint selfUin, ProtoTreeRoot root)
    {
        var pb = root.PathTo<ProtoTreeRoot>("0A");
        {
            var seq = (uint) pb.GetLeafVar("28");
            var time = (uint) pb.GetLeafVar("30");
            var groupCode = (uint) pb.GetLeafVar("08");
            var memberUin = (uint) pb.GetLeafVar("78");
            var memberNick = pb.GetLeafString("8201");

            var e = GroupMemberIncreaseEvent.Push(Oicq.GroupCode2GroupUin(groupCode), memberUin, memberNick);
            e.SetFilterIdenfidentor(time, seq);

            return e;
        }
    }
}
