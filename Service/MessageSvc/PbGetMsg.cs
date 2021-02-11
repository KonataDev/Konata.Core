using System;
using System.Collections.Generic;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packet;
using Konata.Core.Packet.Protobuf;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;
using Konata.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Service.MessageSvc
{
    [Service("MessageSvc.PbGetMsg", "Get message")]
    [EventDepends(typeof(PrivateMessageEvent))]
    [EventDepends(typeof(PrivateMessagePullEvent))]
    internal class PbGetMsg : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            var message = new PrivateMessageEvent();
            {
                var root = ProtoTreeRoot.Deserialize(input.Payload, true);
                {
                    // Get sync cookie 
                    message.SyncCookie = ((ProtoLengthDelimited)(ProtoTreeRoot)root.PathTo("1A")).Value;

                    var sourceRoot = (ProtoTreeRoot)root.PathTo("2A.22.0A");
                    {
                        message.FriendUin = (uint)sourceRoot.GetLeafVar("08");
                    }

                    var sliceInfoRoot = (ProtoTreeRoot)root.PathTo("2A.22.12");
                    {
                        message.SliceTotal = (uint)sliceInfoRoot.GetLeafVar("08");
                        message.SliceIndex = (uint)sliceInfoRoot.GetLeafVar("10");
                        message.SliceFlags = (uint)sliceInfoRoot.GetLeafVar("18");
                    }

                    var contentRoot = (ProtoTreeRoot)root.PathTo("2A.22.1A.0A");
                    {
                        List<MessageChain> list = new List<MessageChain>();

                        contentRoot.ForEach((_, __) =>
                        {
                            if (_ == "12")
                            {
                                ((ProtoTreeRoot)__).ForEach((key, value) =>
                                {
                                    MessageChain chain = null;
                                    try
                                    {
                                        switch (key)
                                        {
                                            case "0A":
                                                chain = ParsePlainText((ProtoTreeRoot)value);
                                                break;

                                            case "12":
                                                chain = ParseQFace((ProtoTreeRoot)value);
                                                break;

                                            case "22":
                                                chain = ParsePicture((ProtoTreeRoot)value);
                                                break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message, e.StackTrace);
                                    }

                                    if (chain != null)
                                    {
                                        list.Add(chain);
                                    }
                                });
                            }
                        });

                        message.Message = list;
                        message.SessionSequence = input.Sequence;
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
        private MessageChain ParsePicture(ProtoTreeRoot tree)
        {
            return new ImageChain
            {
                ImageUrl = tree.GetLeafString("D201"),
                FileHash = ByteConverter.Hex(tree.GetLeafBytes("3A")),
                FileName = tree.GetLeafString("0A")
            };
        }

        /// <summary>
        /// Process Text
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private MessageChain ParsePlainText(ProtoTreeRoot tree)
        {
            // Plain text chain
            if (tree.TryGetLeafString("0A", out var content))
            {
                return new PlainTextChain { Content = content };
            }

            return null;
        }

        /// <summary>
        /// Process QFace chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private MessageChain ParseQFace(ProtoTreeRoot tree)
        {
            return new QFaceChain
            {
                FaceId = (uint)tree.GetLeafVar("08")
            };
        }

        public bool Build(Sequence sequence, PrivateMessagePullEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var pullRequest = new PrivateMsgPullRequest(input.SyncCookie);

            if (SSOFrame.Create("MessageSvc.PbGetMsg", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(pullRequest), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.UinInfo.Uin, signInfo.D2Token, signInfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
            => Build(sequence, (PrivateMessagePullEvent)input, signInfo, out newSequence, out output);
    }
}
