using System.Linq;
using System.Text;
using System.Collections.Generic;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Message.Model;

namespace Konata.Core.Message;

internal static class MessagePacker
{
    /// <summary>
    /// Pack up message chain to byte
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] PackUp(MessageChain input)
    {
        var root = new ProtoTreeRoot();
        foreach (var chain in input.Chains)
        {
            switch (chain)
            {
                case TextChain textChain:
                    ConstructText(root, textChain);
                    break;

                case AtChain atChain:
                    ConstructAt(root, atChain);
                    break;

                case QFaceChain qfaceChain:
                    ConstructQFace(root, qfaceChain);
                    break;

                case ImageChain imageChain:
                    ConstructImage(root, imageChain);
                    break;

                case RecordChain recordChain:
                    ConstructRecord(root, recordChain);
                    break;

                case XmlChain xmlChain:
                    ConstructXml(root, xmlChain);
                    break;

                case JsonChain jsonChain:
                    ConstructJson(root, jsonChain);
                    break;
            }
        }

        return ProtoTreeRoot.Serialize(root).GetBytes();
    }

    /// <summary>
    /// Pack up multi msg to byte
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] PackMultiMsg(List<(SourceInfo, MessageChain)> input)
    {
        var newClientMsgs = new ProtoTreeRoot();
        var compatiableMsgs = new ProtoTreeRoot();
        var multiMsgExpanded = new ProtoTreeRoot();

        foreach (var (source, chain) in input)
        {
            // Check if multimsg chain
            var isMultiMsg = chain.Count == 1 &&
                             chain.FirstOrDefault() is MultiMsgChain;

            compatiableMsgs.AddTree("0A", _ =>
            {
                // Message source
                _.AddTree("0A", __ => ConstructSource(__, source));

                // Message content
                _.AddTree("1A", __ => __.AddLeafBytes("0A", isMultiMsg
                    ? PackUp(new(TextChain.Create("[合并转发]请升级新版本查看")))
                    : PackUp(chain)));
            });

            newClientMsgs.AddTree("0A", _ =>
            {
                // Message source
                _.AddTree("0A", __ => ConstructSource(__, source));

                // Message content
                _.AddTree("1A", __ => __.AddLeafBytes("0A", PackUp(chain)));
            });

            if (isMultiMsg)
            {
                var multiMsg = chain.GetChain<MultiMsgChain>();
                multiMsgExpanded.AddTree("12", _ =>
                {
                    _.AddLeafString("0A", multiMsg.Guid);
                    _.AddTree("12", __ =>
                    {
                        foreach (var subchain in multiMsg.Messages)
                        {
                            __.AddTree("0A", ___ =>
                            {
                                ___.AddTree("0A", ____ => ConstructSource(____, subchain.info));
                                ___.AddTree("1A", ____ => ____.AddLeafBytes("0A", PackUp(subchain.chain)));
                            });
                        }
                    });
                });
            }
        }

        // Construct multimsg tree
        var tree = new ProtoTreeRoot();
        tree.AddTree(compatiableMsgs);
        tree.AddTree(multiMsgExpanded);
        tree.AddTree("12", _ =>
        {
            _.AddLeafString("0A", "MultiMsg");
            _.AddTree("12", newClientMsgs);
        });

        return ProtoTreeRoot.Serialize(tree).GetBytes();
    }

    public static MessageChain UnPack(ProtoTreeRoot root)
    {
        BaseChain chain;
        var builder = new MessageBuilder();

        root.ForEach((_, __) =>
        {
            switch (_)
            {
                // Messages
                case "12":
                    ((ProtoTreeRoot) __).ForEach((key, value) =>
                    {
                        chain = key switch
                        {
                            "0A" => ParseText((ProtoTreeRoot) value),
                            "12" => ParseQFace((ProtoTreeRoot) value),
                            "42" => ParsePicture((ProtoTreeRoot) value),
                            "62" => ParseXml((ProtoTreeRoot) value),
                            "9A01" => ParseShortVideo((ProtoTreeRoot) value),
                            "9A03" => ParseJson((ProtoTreeRoot) value),
                            "EA02" => ParseReply((ProtoTreeRoot) value),
                            _ => null
                        };

                        if (chain != null) builder.Add(chain);
                    });
                    break;

                // Audio message
                case "22":
                {
                    chain = ParseRecord((ProtoTreeRoot) __);
                    if (chain != null) builder.Add(chain);
                    break;
                }
            }
        });

        return builder.Build();
    }

    private static void ConstructSource(ProtoTreeRoot root, SourceInfo source)
    {
        // Message source
        root.AddLeafVar("08", source.SourceUin); // Source uin
        root.AddLeafVar("18", 82); // Type
        root.AddLeafVar("28", source.MessageId); // Sequence
        root.AddLeafVar("30", source.MessageTime); // Time stamp
        root.AddLeafVar("38", source.MessageUniSeq); // Uniseq

        // Multimsg from group
        if (true)
        {
            root.AddTree("4A", _ =>
            {
                _.AddLeafVar("08", source.SourceUin);
                _.AddLeafString("22", source.SourceName);
            });
        }

        // __.AddLeafString("38", 82); // Name
        // __.AddTree("A201", ___ =>
        // { 
        //     ___.AddLeafVar("08", 0);
        //     ___.AddLeafBytes("10", null);
        // });
    }

    private static void ConstructPBReserved(ProtoTreeRoot root, int v8801, int v78)
    {
        root.AddTree("12", (_) =>
        {
            _.AddTree("AA02", (__) =>
            {
                __.AddLeafVar("8801", v8801);
                __.AddTree("9A01", (___) =>
                {
                    ___.AddLeafVar("78", v78);
                    ___.AddLeafVar("F801", 0);
                    ___.AddLeafVar("C802", 0);
                });
            });
        });
    }

    private static void ConstructJson(ProtoTreeRoot root, JsonChain chain)
    {
        // Compress the content
        var deflate = Compression.ZCompress(chain.Content);
        var compressed = new byte[1 + deflate.Length];
        {
            compressed[0] = 0x01;
            deflate.CopyTo(compressed, 1);
        }

        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("AA03", (_) =>
            {
                _.AddLeafVar("08", 0x14);
                _.AddTree("12", __ => __.AddLeafBytes("0A", compressed));
                _.AddLeafVar("18", 0x01);
            });
        });

        ConstructPBReserved(root, 2153, 116581);
    }

    private static void ConstructXml(ProtoTreeRoot root, XmlChain chain)
    {
        // Compress the content
        var deflate = Compression.ZCompress(chain.Content);
        var compressed = new byte[1 + deflate.Length];
        {
            compressed[0] = 0x01;
            deflate.CopyTo(compressed, 1);
        }

        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("62", (_) =>
            {
                _.AddLeafBytes("0A", compressed);
                _.AddLeafVar("10", 0x23);
            });
        });

        ConstructPBReserved(root, 0, 65536);
    }

    private static void ConstructText(ProtoTreeRoot root, TextChain chain)
    {
        // @formatter:off
        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("0A", (_) =>
            {
                _.AddLeafString("0A", chain.Content);
            });
        });
        // @formatter:on
    }

    private static void ConstructAt(ProtoTreeRoot root, AtChain chain)
    {
        // Construct parameters
        var data = new ByteBuffer();
        {
            data.PutByte(0x00);
            data.PutUintLE(1);
            data.PutByte((byte) chain.DisplayString.Length);
            data.PutByte((byte) (chain.AtUin == 0 ? 0x01 : 0x00));
            data.PutUintBE(chain.AtUin);
            data.PutShortBE(0x0000);
        }

        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("0A", (_) =>
            {
                _.AddLeafString("0A", chain.DisplayString);
                _.AddLeafBytes("1A", data.GetBytes());
            });
        });
    }

    private static void ConstructRecord(ProtoTreeRoot root, RecordChain chain)
    {
        root.AddTree("22", (_) =>
        {
            _.AddLeafVar("08", 4);
            _.AddLeafVar("10", chain.SelfUin);
            _.AddLeafBytes("22", chain.HashData);
            _.AddLeafString("2A", chain.FileName);
            _.AddLeafVar("30", chain.FileLength);
            _.AddLeafVar("40", chain.PttUpInfo.UploadId);
            _.AddLeafVar("58", 1);
            _.AddLeafString("9201", chain.PttUpInfo.FileKey);
            _.AddLeafVar("9801", chain.TimeSeconds);
            _.AddLeafVar("E801", 1);
            _.AddTree("F201", __ =>
            {
                __.AddLeafVar("08", 0);
                __.AddLeafVar("28", 0);
                __.AddLeafVar("38", 0);
            });
        });
    }

    private static void ConstructImage(ProtoTreeRoot root, ImageChain chain)
    {
        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("42", (_) =>
            {
                _.AddLeafString("12", chain.FileName);
                _.AddLeafVar("38", chain.PicUpInfo.Ip);
                _.AddLeafVar("40", chain.PicUpInfo.UploadId);
                _.AddLeafVar("48", chain.PicUpInfo.Port);
                _.AddLeafVar("50", 66);
                //_.AddLeafString("5A", "e3vEdCESKrkycKTZ"); // TODO: Unknown
                _.AddLeafVar("60", 1);
                _.AddLeafBytes("6A", chain.HashData);
                _.AddLeafVar("A001", (long) chain.ImageType);
                _.AddLeafVar("B001", chain.Width);
                _.AddLeafVar("B801", chain.Height);
                _.AddLeafVar("C001", 200); // TODO: Unknown
                _.AddLeafVar("C801", chain.FileLength);
                _.AddLeafVar("D001", 0);
                _.AddLeafVar("E801", 0);
                _.AddLeafVar("F001", 0);
                _.AddTree("9202", __ => __.AddLeafVar("78", 2));
            });
        });
    }

    private static void ConstructQFace(ProtoTreeRoot root, QFaceChain chain)
    {
        // @formatter:off
        root.AddTree("12", (leaf) =>
        {
            leaf.AddTree("12", (_) =>
            {
                _.AddLeafVar("08", chain.FaceId);
            });
        });
        // @formatter:on
    }

    /// <summary>
    /// Process json chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static BaseChain ParseJson(ProtoTreeRoot tree)
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
    private static BaseChain ParseXml(ProtoTreeRoot tree)
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
            url = "http://grouptalk.c2c.qq.com" + url;

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
}
