using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MessageSvc;

[Service("MessageSvc.PbGetMsg", "Get message")]
[EventSubscribe(typeof(PbGetMessageEvent))]
internal class PbGetMsg : BaseService<PbGetMessageEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PbGetMessageEvent output)
    {
        var root = ProtoTreeRoot.Deserialize(input.Payload, true);

        // Get sync cookie 
        root.TryGetLeafBytes("1A", out var cookie);

        // Get push events
        var push = new List<ProtocolEvent>();

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
                    var type = (NotifyType) _.GetLeafVar("18");
                    switch (type)
                    {
                        case NotifyType.FriendMessage:
                        case NotifyType.FriendMessageSingle:
                        case NotifyType.FriendPttMessage:
                        case NotifyType.StrangerMessage:
                            push.Add(ProcessMessage(j));
                            break;

                        default:
                        case NotifyType.FriendFileMessage:
                        case NotifyType.NewMember:
                        case NotifyType.GroupCreated:
                        case NotifyType.GroupRequestAccepted:
                            break;
                    }
                });
            }
        }

        Finish:
        output = PbGetMessageEvent.Result(0, cookie, push);
        return true;
    }

    private FriendMessageEvent ProcessMessage(ProtoTreeRoot root)
    {
        var output = FriendMessageEvent.Push();
        {
            var sourceRoot = (ProtoTreeRoot) root.PathTo("0A");
            {
                output.SetFriendUin((uint) sourceRoot.GetLeafVar("08"));
            }

            var contentRoot = (ProtoTreeRoot) root.PathTo("1A.0A");
            {
                var builder = new MessageBuilder();

                contentRoot.ForEach((_, __) =>
                {
                    if (_ != "12") return;

                    ((ProtoTreeRoot) __).ForEach((key, value) =>
                    {
                        BaseChain chain = null;
                        try
                        {
                            switch (key)
                            {
                                case "0A":
                                    chain = ParseText((ProtoTreeRoot) value);
                                    break;

                                case "12":
                                    chain = ParseQFace((ProtoTreeRoot) value);
                                    break;

                                case "22":
                                    chain = ParsePicture((ProtoTreeRoot) value);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message, e.StackTrace);
                        }

                        if (chain != null)
                        {
                            builder.Add(chain);
                        }
                    });
                });

                output.SetMessage(builder.Build());
                // output.SetSessionSequence(seq);
            }
        }

        return output;
    }

    /// <summary>
    /// Process Picture chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private BaseChain ParsePicture(ProtoTreeRoot tree)
    {
        // TODO: fix args
        var hash = ByteConverter.Hex(tree.GetLeafBytes("3A"));
        return ImageChain.Create(
            "https://c2cpicdw.qpic.cn" + tree.GetLeafString("7A"),
            hash, hash,
            (uint) tree.GetLeafVar("48"), (uint) tree.GetLeafVar("40"), (uint) tree.GetLeafVar("10"),
            ImageType.JPG);
    }

    /// <summary>
    /// Process Text
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private BaseChain ParseText(ProtoTreeRoot tree)
        => TextChain.Create(tree.GetLeafString("0A"));

    /// <summary>
    /// Process QFace chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private BaseChain ParseQFace(ProtoTreeRoot tree)
        => QFaceChain.Create((uint) tree.GetLeafVar("08"));


    protected override bool Build(Sequence sequence, PbGetMessageEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var pullRequest = new GetMessageRequest(input.SyncCookie);

        if (SSOFrame.Create("MessageSvc.PbGetMsg", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(pullRequest), out var ssoFrame))
        {
            if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
            {
                return ServiceMessage.Build(toService, device, out output);
            }
        }

        return false;
    }
}
