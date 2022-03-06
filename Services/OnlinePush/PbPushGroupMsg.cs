using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.OnlinePush
{
    [Service("OnlinePush.PbPushGroupMsg", "Receive group message from server")]
    [EventSubscribe(typeof(GroupMessageEvent))]
    public class PbPushGroupMsg : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            var message = GroupMessageEvent.Result(0);
            {
                var root = ProtoTreeRoot.Deserialize(input.Payload, true);
                {
                    // Parse message source information
                    var sourceRoot = (ProtoTreeRoot) root.PathTo("0A.0A");
                    {
                        message.SetMemberUin((uint) sourceRoot.GetLeafVar("08"));
                        message.SetMessageId((uint) sourceRoot.GetLeafVar("28"));
                        message.SetMessageTime((uint) sourceRoot.GetLeafVar("30"));
                        message.SetMessageUniSeq((uint) sourceRoot.GetLeafVar("38"));

                        sourceRoot = (ProtoTreeRoot) sourceRoot.PathTo("4A");
                        {
                            message.SetGroupUin((uint) sourceRoot.GetLeafVar("08"));
                            message.SetGroupName(sourceRoot.GetLeafString("42"));

                            // Try get member card
                            if (sourceRoot.TryGetLeafString("22", out var cardText))
                            {
                                message.SetMemberCard(cardText);
                            }
                            else
                            {
                                // This member card contains a color code
                                // We need to ignore this
                                sourceRoot = (ProtoTreeRoot) sourceRoot.PathTo("22");
                                if (sourceRoot.GetLeaves("0A").Count == 2)
                                {
                                    message.SetMemberCard(((ProtoLengthDelimited)
                                        sourceRoot.PathTo("0A[1].12")).ToString());
                                }
                            }
                        }
                    }

                    // Parse message slice information
                    var sliceInfoRoot = (ProtoTreeRoot) root.PathTo("0A.12");
                    {
                        var total = (uint) sliceInfoRoot.GetLeafVar("08");
                        var index = (uint) sliceInfoRoot.GetLeafVar("10");
                        var flags = (uint) sliceInfoRoot.GetLeafVar("18");
                        message.SetSliceInfo(total, index, flags);
                    }

                    // Parse message content
                    var contentRoot = (ProtoTreeRoot) root.PathTo("0A.1A.0A");
                    {
                        var builder = new MessageBuilder();

                        contentRoot.ForEach((_, __) =>
                        {
                            BaseChain chain = null;

                            // Messages
                            if (_ == "12")
                            {
                                ((ProtoTreeRoot) __).ForEach((key, value) =>
                                {
                                    try
                                    {
                                        chain = key switch
                                        {
                                            "0A" => ParseText((ProtoTreeRoot) value),
                                            "12" => ParseQFace((ProtoTreeRoot) value),
                                            "42" => ParsePicture((ProtoTreeRoot) value),
                                            "62" => ParseXML((ProtoTreeRoot) value),
                                            "9A01" => ParseShortVideo((ProtoTreeRoot) value),
                                            "9A03" => ParseJSON((ProtoTreeRoot) value),
                                            "EA02" => ParseReply((ProtoTreeRoot) value),
                                            _ => null
                                        };
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

                            // Audio message
                            else if (_ == "22")
                            {
                                try
                                {
                                    chain = ParseRecord((ProtoTreeRoot) __);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message, e.StackTrace);
                                }

                                if (chain != null)
                                {
                                    builder.Add(chain);
                                }
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
        /// Process json chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseJSON(ProtoTreeRoot tree)
        {
            var bytes = tree.GetLeafBytes("0A");
            var json = Compression.ZDecompress(bytes[1..]);
            return JsonChain.Create(Encoding.UTF8.GetString(json));
        }

        /// <summary>
        /// Process xml chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseXML(ProtoTreeRoot tree)
        {
            var bytes = tree.GetLeafBytes("0A");
            var xml = Compression.ZDecompress(bytes[1..]);
            return XmlChain.Create(Encoding.UTF8.GetString(xml));
        }

        /// <summary>
        /// Process reply chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseReply(ProtoTreeRoot tree)
        {
            var messageId = (uint) tree.GetLeafVar("08");
            var replyUin = (uint) tree.GetLeafVar("10");
            var replyTime = (uint) tree.GetLeafVar("18");

            // TODO:
            // Parse original chain 0x2A

            return ReplyChain.Create(messageId, replyUin, replyTime);
        }

        /// <summary>
        /// Process short video chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseShortVideo(ProtoTreeRoot tree)
        {
            var width = (uint) tree.GetLeafVar("38");
            var height = (uint) tree.GetLeafVar("40");
            var hashstr = ByteConverter.Hex(tree.GetLeafBytes("12"));
            var storage = tree.GetLeafString("1A");
            var duration = (uint) tree.GetLeafVar("28");

            return VideoChain.Create(hashstr, hashstr,
                storage, width, height, duration);
        }

        /// <summary>
        /// Process record chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseRecord(ProtoTreeRoot tree)
        {
            var url = tree.GetLeafString("A201");
            var hashstr = ByteConverter.Hex(tree.GetLeafBytes("22"));

            if (!url.StartsWith("http"))
            {
                url = "http://grouptalk.c2c.qq.com" + url;
            }

            return RecordChain.Create(url, hashstr, hashstr);
        }

        /// <summary>
        /// Process image chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParsePicture(ProtoTreeRoot tree)
        {
            var url = tree.GetLeafString("8201");
            var hashstr = ByteConverter.Hex(tree.GetLeafBytes("6A"));

            var width = (uint) tree.GetLeafVar("B001");
            var height = (uint) tree.GetLeafVar("B801");
            var length = (uint) tree.GetLeafVar("C801");
            var imgtype = ImageType.JPG;

            // hmm not sure
            if (tree.TryGetLeafVar("A001", out var type))
            {
                imgtype = (ImageType) type;
            }

            else
            {
                // Try get image type
                // from file extension
                var split = tree.GetLeafString("12").Split('.');

                if (split.Length == 2)
                {
                    imgtype = split[1] switch
                    {
                        "jpg" => ImageType.JPG,
                        "png" => ImageType.PNG,
                        "bmp" => ImageType.BMP,
                        "gif" => ImageType.GIF,
                        "webp" => ImageType.WEBP,
                        _ => ImageType.JPG
                    };
                }
            }

            // Create image chain
            return ImageChain.Create(url, hashstr,
                hashstr, width, height, length, imgtype);
        }

        /// <summary>
        /// Process Text and At chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseText(ProtoTreeRoot tree)
        {
            // At chain
            if (tree.TryGetLeafBytes("1A", out var leaf))
            {
                var at = ByteConverter.BytesToUInt32(leaf
                    .Skip(7).Take(4).ToArray(), 0, Endian.Big);

                return AtChain.Create(at);
            }

            // Plain text chain
            return TextChain.Create(tree.GetLeafString("0A"));
        }

        /// <summary>
        /// Process QFace chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseQFace(ProtoTreeRoot tree)
            => QFaceChain.Create((uint) tree.GetLeafVar("08"));

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}
