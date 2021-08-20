using System;
using System.Linq;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
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
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            var message = new GroupMessageEvent();
            {
                var root = ProtoTreeRoot.Deserialize(input.Payload, true);
                {
                    // Parse message source information
                    var sourceRoot = (ProtoTreeRoot) root.PathTo("0A.0A");
                    {
                        message.MemberUin = (uint) sourceRoot.GetLeafVar("08");
                        message.MessageId = (uint) sourceRoot.GetLeafVar("28");
                        message.MessageTime = (uint) sourceRoot.GetLeafVar("30");

                        sourceRoot = (ProtoTreeRoot) sourceRoot.PathTo("4A");
                        {
                            message.GroupUin = (uint) sourceRoot.GetLeafVar("08");
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
                                sourceRoot = (ProtoTreeRoot) sourceRoot.PathTo("22");
                                if (sourceRoot.GetLeaves("0A").Count == 2)
                                {
                                    message.MemberCard = ((ProtoLengthDelimited) sourceRoot.PathTo("0A[1].12")).ToString();
                                }
                            }
                        }
                    }

                    // Parse message slice information
                    var sliceInfoRoot = (ProtoTreeRoot) root.PathTo("0A.12");
                    {
                        message.SliceTotal = (uint) sliceInfoRoot.GetLeafVar("08");
                        message.SliceIndex = (uint) sliceInfoRoot.GetLeafVar("10");
                        message.SliceFlags = (uint) sliceInfoRoot.GetLeafVar("18");
                    }

                    // Parse message content
                    var contentRoot = (ProtoTreeRoot) root.PathTo("0A.1A.0A");
                    {
                        var list = new MessageChain();

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
                                            "0A" => ParsePlainText((ProtoTreeRoot) value),
                                            "12" => ParseQFace((ProtoTreeRoot) value),
                                            "42" => ParsePicture((ProtoTreeRoot) value),
                                            "9A01" => ParseShortVideo((ProtoTreeRoot) value),
                                            _ => null
                                        };
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
                                    list.Add(chain);
                                }
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
        private static BaseChain ParsePlainText(ProtoTreeRoot tree)
        {
            // At chain
            if (tree.TryGetLeafBytes("1A", out var atBytes))
            {
                var at = ByteConverter.BytesToUInt32
                    (atBytes.Skip(7).Take(4).ToArray(), 0, Endian.Big);

                return AtChain.Create(at);
            }

            // Plain text chain
            if (tree.TryGetLeafString("0A", out var content))
            {
                return PlainTextChain.Create(content);
            }

            return null;
        }

        /// <summary>
        /// Process QFace chain
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static BaseChain ParseQFace(ProtoTreeRoot tree)
            => QFaceChain.Create((uint) tree.GetLeafVar("08"));

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}
