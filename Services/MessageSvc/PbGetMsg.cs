using System;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PbGetMsg", "Get message")]
    [EventSubscribe(typeof(PrivateMessageEvent))]
    [EventSubscribe(typeof(PrivateMessagePullEvent))]
    internal class PbGetMsg : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            var message = PrivateMessageEvent.Push();
            {
                var root = ProtoTreeRoot.Deserialize(input.Payload, true);
                {
                    // Get sync cookie 
                    message.SetSyncCookie(((ProtoLengthDelimited)
                        (ProtoTreeRoot) root.PathTo("1A")).Value);

                    var sourceRoot = (ProtoTreeRoot) root.PathTo("2A.22.0A");
                    {
                        message.SetFriendUin((uint) sourceRoot.GetLeafVar("08"));
                    }

                    var sliceInfoRoot = (ProtoTreeRoot) root.PathTo("2A.22.12");
                    {
                        var total = (uint) sliceInfoRoot.GetLeafVar("08");
                        var index = (uint) sliceInfoRoot.GetLeafVar("10");
                        var flags = (uint) sliceInfoRoot.GetLeafVar("18");
                        message.SetSliceInfo(total, index, flags);
                    }

                    var contentRoot = (ProtoTreeRoot) root.PathTo("2A.22.1A.0A");
                    {
                        var builder = new MessageBuilder();

                        contentRoot.ForEach((_, __) =>
                        {
                            if (_ == "12")
                            {
                                ((ProtoTreeRoot) __).ForEach((key, value) =>
                                {
                                    BaseChain chain = null;
                                    try
                                    {
                                        switch (key)
                                        {
                                            case "0A":
                                                chain = ParsePlainText((ProtoTreeRoot) value);
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
                            }
                        });

                        message.SetMessage(builder.Build());
                        message.SetSessionSequence(input.Sequence);
                    }
                }
            }

            output = message;
            return true;
        }

        /// <summary>
        /// Process Picture chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private BaseChain ParsePicture(ProtoTreeRoot tree)
        {
            // TODO: fix args
            return ImageChain.Create(
                tree.GetLeafString("D201"),
                ByteConverter.Hex(tree.GetLeafBytes("3A")),
                tree.GetLeafString("0A"), 0, 0, 0, ImageType.JPG);
        }

        /// <summary>
        /// Process Text
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private BaseChain ParsePlainText(ProtoTreeRoot tree)
            => PlainTextChain.Create(tree.GetLeafString("0A"));

        /// <summary>
        /// Process QFace chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private BaseChain ParseQFace(ProtoTreeRoot tree)
            => QFaceChain.Create((uint) tree.GetLeafVar("08"));

        public bool Build(Sequence sequence, PrivateMessagePullEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var pullRequest = new PrivateMsgPullRequest(input.SyncCookie);

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

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (PrivateMessagePullEvent) input, keystore, device, out newSequence, out output);
    }
}
