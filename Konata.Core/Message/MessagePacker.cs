using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Konata.Core.Message;

internal static class MessagePacker
{
    /// <summary>
    /// Pack up message chain to byte
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static byte[] PackUp(MessageChain input, Mode mode)
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
                    ConstructImage(root, imageChain, mode);
                    break;

                case RecordChain recordChain:
                    ConstructRecord(root, recordChain, mode);
                    break;

                case XmlChain xmlChain:
                    ConstructXml(root, xmlChain);
                    break;

                case JsonChain jsonChain:
                    ConstructJson(root, jsonChain);
                    break;

                case ReplyChain replyChain:
                    ConstructReply(root, replyChain);
                    break;
            }
        }

        return ProtoTreeRoot.Serialize(root).GetBytes();
    }

    /// <summary>
    /// Pack up multi msg to byte
    /// </summary>
    /// <param name="main"></param>
    /// <param name="sides"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static byte[] PackMultiMsg(MultiMsgChain main, List<MultiMsgChain> sides, Mode mode)
    {
        var newClientMsgs = new ProtoTreeRoot();
        var compatiableMsgs = new ProtoTreeRoot();
        var flatMultiMsgs = new ProtoTreeRoot();

        // For old device compatibility
        compatiableMsgs.AddTree("0A", _ =>
        {
            // Message source
            _.AddTree("0A", __ => ConstructSource(__, main.Messages[0]));

            // Message content
            _.AddTree("1A", __ => __.AddLeafBytes("0A",
                PackUp(new(TextChain.Create("[合并转发]请升级新版本查看")), mode)));
        });

        // Construct root multimsg
        foreach (var i in main)
        {
            newClientMsgs.AddTree("0A", _ =>
            {
                // Message source
                _.AddTree("0A", __ => ConstructSource(__, i));

                // Message content
                _.AddTree("1A", __ => __.AddLeafBytes("0A", PackUp(i.Chain, mode)));
            });
        }

        // Construct multimsg reference table
        if (sides.Count > 0)
        {
            foreach (var i in sides)
            {
                flatMultiMsgs.AddTree("12", _ =>
                {
                    _.AddLeafString("0A", i.FileName);
                    _.AddTree("12", __ =>
                    {
                        foreach (var subchain in i.Messages)
                        {
                            __.AddTree("0A", ___ =>
                            {
                                ___.AddTree("0A", ____ => ConstructSource(____, subchain));
                                ___.AddTree("1A", ____ => ____.AddLeafBytes("0A", PackUp(subchain.Chain, mode)));
                            });
                        }
                    });
                });
            }
        }

        // Construct multimsg tree
        var tree = new ProtoTreeRoot();
        tree.AddTree(compatiableMsgs);
        tree.AddTree(flatMultiMsgs);
        tree.AddTree("12", _ =>
        {
            _.AddLeafString("0A", "MultiMsg");
            _.AddTree("12", newClientMsgs);
        });

        return ProtoTreeRoot.Serialize(tree).GetBytes();
    }

    /// <summary>
    /// Pack up multi msg to byte
    /// </summary>
    /// <param name="root"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static MessageChain UnPack(ProtoTreeRoot root, Mode mode)
    {
        BaseChain chain;
        var builder = new MessageBuilder();

        var isFileEvent = root.TryGetLeaf("12", out var fileLeaf);
        if (isFileEvent) builder.Add(ParseFile(fileLeaf as ProtoTreeRoot, mode));

        (root.GetLeaf<ProtoTreeRoot>("0A")).ForEach<ProtoTreeRoot>((key, val) =>
        {
            switch (key)
            {
                // Messages
                case "12":
                    (val).ForEach<ProtoTreeRoot>((subkey, subval) =>
                    {
                        chain = subkey switch
                        {
                            "0A" => ParseTextOrAt(subval),
                            "12" => ParseQFace(subval),
                            "42" => ParseImage(subval, Mode.Group),
                            "62" => ParseXml(subval),
                            "9A01" => ParseShortVideo(subval),
                            "9A03" => ParseJson(subval),
                            "EA02" => ParseReply(subval),

                            "AA03" => ParseCommonElem(subval),

                            // Friend image
                            "22" => ParseImage(subval, Mode.Friend),

                            _ => null
                        };

                        if (chain != null) builder.Add(chain);
                    });
                    break;

                // Audio message
                case "22":
                {
                    chain = ParseRecord(val, mode);
                    if (chain != null) builder.Add(chain);
                    break;
                }
            }
        });

        return builder.Build();
    }

    private static void ConstructSource(ProtoTreeRoot root, MessageStruct source)
    {
        // Message source
        root.AddLeafVar("08", source.Sender.Uin); // Source uin
        root.AddLeafVar("18", 82); // Type
        root.AddLeafVar("28", source.Sequence); // Sequence
        root.AddLeafVar("30", source.Time); // Time stamp
        root.AddLeafVar("38", source.Uuid); // Uniseq

        // Multimsg from group
        if (source.Type == MessageStruct.SourceType.Group)
        {
            root.AddTree("4A", _ =>
            {
                _.AddLeafVar("08", source.Sender.Uin);
                _.AddLeafString("22", source.Sender.Name);
            });
        }

        // __.AddLeafString("38", 82); // Name
        root.AddTree("A201", _ =>
        {
            _.AddLeafVar("08", 0);
            _.AddLeafVar("10", source.Uuid);
        });
    }

    private static void ConstructPbReserved(ProtoTreeRoot root, int v8801, int v78)
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

        ConstructPbReserved(root, 2153, 116581);
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

        ConstructPbReserved(root, 0, 65536);
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

    private static void ConstructReply(ProtoTreeRoot root, ReplyChain chain)
    {
        root.AddTree("12", (_) =>
        {
            _.AddTree("EA02", __ =>
            {
                __.AddLeafVar("08", chain.Sequence);
                __.AddLeafVar("10", chain.Uin);
                __.AddLeafVar("18", chain.Time);
                __.AddLeafVar("20", 1);

                __.AddTree("2A", ___ => ___.AddTree("0A",
                    ____ => ____.AddLeafString("0A", chain.Preview)));

                __.AddLeafVar("30", 0);

                __.AddTree("42", ___ =>
                {
                    ___.AddLeafVar("10", 1);
                    ___.AddLeafVar("18", chain.Uuid);
                    ___.AddTree("2A", ____ =>
                    {
                        ____.AddLeafVar("08", chain.Sequence);
                        ____.AddLeafVar("10", chain.Sequence);
                        ____.AddLeafVar("18", chain.Sequence);
                    });
                });

                __.AddLeafVar("50", 0);
            });
        });
    }

    private static void ConstructRecord(ProtoTreeRoot root, RecordChain chain, Mode mode)
    {
        root.AddTree("22", (_) =>
        {
            _.AddLeafVar("08", 4);
            _.AddLeafVar("10", chain.SelfUin);
            _.AddLeafBytes("22", chain.HashData);
            _.AddLeafString("2A", chain.FileName);
            _.AddLeafVar("30", chain.FileLength);
            _.AddLeafVar("58", 1);

            if (mode == Mode.Group)
            {
                _.AddLeafVar("40", (uint) chain.PttUpInfo.UploadId);
                _.AddLeafString("9201", chain.PttUpInfo.FileKey);
                _.AddLeafVar("9801", chain.TimeSeconds);
                _.AddLeafVar("E801", 1);
                _.AddTree("F201", __ =>
                {
                    __.AddLeafVar("08", 0);
                    __.AddLeafVar("28", 0);
                    __.AddLeafVar("38", 0);
                });
            }
            else
            {
                _.AddLeafString("1A", chain.PttUpInfo.FileKey);
                _.AddLeafBytes("3A", "0308000400000001090004000000030A0006080028003800".UnHex());
            }
        });
    }

    private static void ConstructImage(ProtoTreeRoot root, ImageChain chain, Mode mode)
    {
        var image = new ProtoTreeRoot();
        {
            if (mode == Mode.Group)
            {
                image.AddLeafString("12", chain.FileName);
                image.AddLeafVar("38", chain.PicUpInfo.Ip);
                image.AddLeafVar("40", (uint) chain.PicUpInfo.UploadId);
                image.AddLeafVar("48", chain.PicUpInfo.Port);
                image.AddLeafVar("50", 66);
                //_.AddLeafString("5A", "e3vEdCESKrkycKTZ"); // TODO: Unknown
                image.AddLeafVar("60", 1);
                image.AddLeafBytes("6A", chain.HashData);
                image.AddLeafVar("A001", (long) chain.ImageType);
                image.AddLeafVar("B001", chain.Width);
                image.AddLeafVar("B801", chain.Height);
                image.AddLeafVar("C001", 200); // TODO: Unknown
                image.AddLeafVar("C801", chain.FileLength);
                image.AddLeafVar("D001", 0);
                image.AddLeafVar("E801", 0);
                image.AddLeafVar("F001", 0);
                image.AddTree("9202", __ => __.AddLeafVar("78", 2));
            }

            else
            {
                image.AddLeafString("0A", chain.FileName);
                image.AddLeafVar("10", chain.FileLength);
                image.AddLeafString("1A", (string) chain.PicUpInfo.UploadId);
                // image.AddLeafBytes("22", );
                image.AddLeafVar("28", (long) chain.ImageType);
                image.AddLeafBytes("3A", chain.HashData);
                image.AddLeafVar("40", chain.Width);
                image.AddLeafVar("48", chain.Height);
                image.AddLeafString("52", (string) chain.PicUpInfo.UploadId);
                // image.AddLeafBytes("5A", );
                image.AddTree("EA01", __ => __.AddLeafVar("08", 0)); // as face
            }
        }

        // Is not a flash image
        if (chain is not FlashImageChain)
        {
            root.AddTree("12", _ => _.AddTree
                (mode == Mode.Group ? "42" : "22", image));
            return;
        }

        var flash = new ProtoTreeRoot();
        {
            flash.AddTree("AA03", _ =>
            {
                _.AddLeafVar("08", 3);
                _.AddTree("12", __ => __.AddTree
                    (mode == Mode.Group ? "0A" : "12", image));
            });
        }
        root.AddTree("12", flash);
        ConstructText(root, TextChain.Create("[闪照]请使用新版手机QQ查看闪照。"));
    }

    private static void ConstructQFace(ProtoTreeRoot root, QFaceChain chain)
    {
        // Legacy qface
        if (!chain.Big && chain.FaceId <= 255)
        {
            root.AddTree("12", _ => _.AddTree
                ("12", __ => __.AddLeafVar("08", chain.FaceId)));
            return;
        }

        root.AddTree("12", _ => _.AddTree("AA03", __ =>
        {
            // Big qface
            if (chain.Big)
            {
                __.AddLeafVar("08", 37);
                __.AddTree("12", ___ =>
                {
                    ___.AddLeafString("0A", "1");
                    ___.AddLeafString("12", chain.BigFaceId);
                    ___.AddLeafVar("18", chain.FaceId);
                    ___.AddLeafVar("20", 1);
                    ___.AddLeafVar("28", 1);
                    ___.AddLeafString("32", "");
                    ___.AddLeafString("3A", chain.FaceName);
                    ___.AddLeafString("42", "");
                    ___.AddLeafVar("48", 1);
                });
                __.AddLeafVar("18", 1);

                ConstructText(root, TextChain.Create($"{chain.FaceName} 请使用最新版手机QQ体验新功能"));
            }

            // New qface
            else
            {
                __.AddLeafVar("08", 33);
                __.AddTree("12", ___ =>
                {
                    ___.AddLeafVar("08", chain.FaceId);
                    ___.AddLeafString("12", chain.FaceName);
                    ___.AddLeafString("1A", chain.FaceName);
                });
                __.AddLeafVar("18", 1);
            }
        }));
    }

    private static BaseChain ParseTextOrAt(ProtoTreeRoot root)
    {
        if (root.TryGetLeafBytes("1A", out var at))
        {
            // Fix issue #111
            if (at.Length > 0) return ParseAt(at);
        }

        return ParseText(root);
    }

    /// <summary>
    /// Process json chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static JsonChain ParseJson(ProtoTreeRoot tree)
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
    private static XmlChain ParseXml(ProtoTreeRoot tree)
    {
        var bytes = tree.GetLeafBytes("0A");
        var xml = bytes[0] == 1 ? Compression.ZDecompress(bytes[1..]) : bytes[1..];
        return XmlChain.Create(Encoding.UTF8.GetString(xml));
    }

    /// <summary>
    /// Process reply chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static ReplyChain ParseReply(ProtoTreeRoot tree)
    {
        var seq = (uint) tree.GetLeafVar("08");
        var uin = (uint) tree.GetLeafVar("10");
        var time = (uint) tree.GetLeafVar("18");
        var uuid = tree.TryPathTo<ProtoVarInt>("42.18", out var x) ? x : 0;
        var preview = tree.PathTo<ProtoLengthDelimited>("2A.0A.0A").ToString();

        return ReplyChain.Create(uin, seq, uuid, time, preview);
    }

    /// <summary>
    /// Process short video chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static VideoChain ParseShortVideo(ProtoTreeRoot tree)
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
    /// <param name="mode"></param>
    /// <returns></returns>
    private static RecordChain ParseRecord(ProtoTreeRoot tree, Mode mode)
    {
        // TODO: Fixme
        // For PC versions <= 909 has no hash str
        // The result of KQ code will be come to
        // [KQ:record=]

        var url = tree.TryGetLeafString("A201", out var x) ? x : "";
        var hashstr = tree.GetLeafBytes("22").ToHex();

        if (mode == Mode.Group)
        {
            if (!url.StartsWith("http"))
                url = "http://grouptalk.c2c.qq.com" + url;
        }

        return RecordChain.Create(url, hashstr, hashstr);
    }

    /// <summary>
    /// Process image chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static BaseChain ParseCommonElem(ProtoTreeRoot tree)
    {
        switch (tree.GetLeafVar("08"))
        {
            // Flash picture
            case 3:
                var picRoot = tree.GetLeaf<ProtoTreeRoot>("12");
                if (picRoot.TryGetLeaf("0A", out var groupPicTree))
                    return ParseFlash((ProtoTreeRoot) groupPicTree, Mode.Group);
                else
                    return ParseFlash(picRoot.GetLeaf<ProtoTreeRoot>("12"), Mode.Friend);

            // Parse qface
            case 33:
                return ParseQFace(tree.GetLeaf<ProtoTreeRoot>("12"));

            // Parse big qface
            case 37:
                return ParseBigQFace(tree.GetLeaf<ProtoTreeRoot>("12"));

            default:
             case 2:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Process image chain
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    private static FlashImageChain ParseFlash(ProtoTreeRoot tree, Mode mode)
        => FlashImageChain.CreateFromImageChain(ParseImage(tree, mode));

    /// <summary>
    /// Process image chain
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    private static ImageChain ParseImage(ProtoTreeRoot tree, Mode mode)
    {
        if (mode == Mode.Friend)
        {
            var hashstr = ByteConverter.Hex(tree.GetLeafBytes("3A"));

            if (tree.TryGetLeafString("7A", out var url))
                url = $"https://c2cpicdw.qpic.cn{url}";
            else if (tree.TryGetLeafString("52", out url))
                url = $"https://c2cpicdw.qpic.cn/offpic_new/0/{url}/0";

            var imgtype = tree.TryGetLeafVar
                ("A001", out var type)
                ? (ImageType) type
                : ImageType.Jpg;

            return ImageChain.Create(
                url ?? "", hashstr, hashstr,
                (uint) tree.GetLeafVar("48"),
                (uint) tree.GetLeafVar("40"),
                (uint) tree.GetLeafVar("10"),
                imgtype);
        }
        else
        {
            var hash = ByteConverter.Hex(tree.GetLeafBytes("6A"));
            var width = (uint) tree.GetLeafVar("B001");
            var height = (uint) tree.GetLeafVar("B801");
            var length = (uint) tree.GetLeafVar("C801");

            var imgtype = tree.TryGetLeafVar
                ("A001", out var type)
                ? (ImageType) type
                : ImageType.Jpg;

            if (tree.TryGetLeafString("8201", out var url))
                url = $"https://gchat.qpic.cn{url}";
            else
                url = $"https://gchat.qpic.cn/gchatpic_new/0/0-0-{hash}/0";

            // Create image chain
            return ImageChain.Create(url, hash,
                hash, width, height, length, imgtype);
        }
    }

    /// <summary>
    /// Process File chain
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    private static FileChain ParseFile(ProtoTreeRoot tree, Mode mode)
    {
        if (mode == Mode.Friend)
        {
            var fileRoot = tree.GetLeaf<ProtoTreeRoot>("0A");

            var filename = fileRoot.GetLeafString("2A");
            var filesize = (ulong)fileRoot.GetLeafVar("30");
            var fileHash = fileRoot.GetLeafBytes("22");
            var fileUuid = fileRoot.GetLeafString("1A");

            return new FileChain(filename, filesize, fileHash, fileUuid);
        }
        else // TODO: Group File
        {
            var filename = "";
            ulong filesize = 0;
            var fileHash = Array.Empty<byte>();
            var fileUuid = "";
            
            return new FileChain(filename, filesize, fileHash, fileUuid);
        }
    }

    /// <summary>
    /// Process Text and At chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static TextChain ParseText(ProtoTreeRoot tree)
        => TextChain.Create(tree.GetLeafString("0A"));

    /// <summary>
    /// Process Text and At chain
    /// </summary>
    /// <param name="leaf"></param>
    /// <returns></returns>
    private static AtChain ParseAt(byte[] leaf)
        => AtChain.Create(ByteConverter.BytesToUInt32(leaf.Skip(7).Take(4).ToArray(), 0, Endian.Big));

    /// <summary>
    /// Process QFace chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static QFaceChain ParseQFace(ProtoTreeRoot tree)
        => QFaceChain.Create((int) tree.GetLeafVar("08"));

    /// <summary>
    /// Process Big QFace chain
    /// </summary>
    /// <param name="tree"></param>
    /// <returns></returns>
    private static QFaceChain ParseBigQFace(ProtoTreeRoot tree)
        => QFaceChain.Create((int) tree.GetLeafVar("18"), true);

    internal enum Mode
    {
        Group,
        Friend
    }
}
