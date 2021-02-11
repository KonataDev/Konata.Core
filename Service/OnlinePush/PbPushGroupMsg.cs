using System;
using System.Collections.Generic;
using System.Linq;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packet;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;
using Konata.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Service.OnlinePush
{
    [Service("OnlinePush.PbPushGroupMsg", "Receive group message from server")]
    [EventDepends(typeof(GroupMessageEvent))]
    public class PbPushGroupMsg : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            var message = new GroupMessageEvent();
            {
                var root = ProtoTreeRoot.Deserialize(input.Payload, true);
                {
                    // Parse message source information
                    var sourceRoot = (ProtoTreeRoot)root.PathTo("0A.0A");
                    {
                        message.MemberUin = (uint)sourceRoot.GetLeafVar("08");
                        message.MessageId = (uint)sourceRoot.GetLeafVar("28");
                        message.MessageTime = (uint)sourceRoot.GetLeafVar("30");

                        sourceRoot = (ProtoTreeRoot)sourceRoot.PathTo("4A");
                        {
                            message.GroupUin = (uint)sourceRoot.GetLeafVar("08");
                            message.GroupName = sourceRoot.GetLeafString("42");

                            // Try get member card
                            if (sourceRoot.TryGetLeafString("22", out var cardText))
                            {
                                message.MemberCard = cardText;
                            }
                            else
                            {
                                // This member card contains a color code
                                // We need to ignore this
                                sourceRoot = (ProtoTreeRoot)sourceRoot.PathTo("22");
                                if (sourceRoot.GetLeaves("0A").Count == 2)
                                {
                                    message.MemberCard = ((ProtoLengthDelimited)sourceRoot.PathTo("0A[1].12")).ToString();
                                }
                            }
                        }
                    }

                    // Parse message slice information
                    var sliceInfoRoot = (ProtoTreeRoot)root.PathTo("0A.12");
                    {
                        message.SliceTotal = (uint)sliceInfoRoot.GetLeafVar("08");
                        message.SliceIndex = (uint)sliceInfoRoot.GetLeafVar("10");
                        message.SliceFlags = (uint)sliceInfoRoot.GetLeafVar("18");
                    }

                    // Parse message content
                    var contentRoot = (ProtoTreeRoot)root.PathTo("0A.1A.0A");
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

                                            case "42":
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
                ImageUrl = tree.GetLeafString("8201"),
                FileHash = ByteConverter.Hex(tree.GetLeafBytes("6A")),
                FileName = tree.GetLeafString("12").Replace("{", "").Replace("}", "").Replace("-", "")
            };
        }

        /// <summary>
        /// Process Text and At chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private MessageChain ParsePlainText(ProtoTreeRoot tree)
        {
            // At chain
            if (tree.TryGetLeafBytes("1A", out var atBytes))
            {
                var at = ByteConverter.BytesToUInt32
                    (atBytes.Skip(7).Take(4).ToArray(), 0, Endian.Big);

                return new AtChain { AtUin = at };
            }

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

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}
