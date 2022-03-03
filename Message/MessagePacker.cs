using System.Collections.Generic;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

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
        {
            foreach (var chain in input.Chains)
            {
                switch (chain)
                {
                    case TextChain textChain:
                        ConstructPlainTextChain(root, textChain);
                        break;

                    case AtChain atChain:
                        ConstructAtChain(root, atChain);
                        break;

                    case QFaceChain qfaceChain:
                        ConstructQFaceChain(root, qfaceChain);
                        break;

                    case ImageChain imageChain:
                        ConstructImageChain(root, imageChain);
                        break;

                    case RecordChain recordChain:
                        ConstructRecordChain(root, recordChain);
                        break;

                    case XmlChain xmlChain:
                        ConstructXmlChain(root, xmlChain);
                        break;

                    case JsonChain jsonChain:
                        ConstructJsonChain(root, jsonChain);
                        break;
                }
            }
        }

        return ProtoTreeRoot.Serialize(root).GetBytes();
    }

    /// <summary>
    /// Pack up multi msg to byte
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] PackMultiMsg(List<(SourceInfo, MessageChain)> input, byte[] guid)
    {
        var msgs = new ProtoTreeRoot();
        foreach (var (source, chain) in input)
        {
            msgs.AddTree("0A", _ =>
            {
                // Message source
                _.AddTree("0A", __ =>
                {
                    __.AddLeafVar("08", source.SourceUin); // Source uin
                    __.AddLeafVar("18", 82); // Type
                    __.AddLeafVar("28", source.MessageId); // Sequence
                    __.AddLeafVar("30", source.MessageTime); // Time stamp
                    __.AddLeafVar("38", source.MessageUniSeq); // Uniseq

                    // Multimsg from group
                    if (true)
                    {
                        __.AddTree("4A", ___ =>
                        {
                            ___.AddLeafVar("08", source.SourceUin);
                            ___.AddLeafString("22", source.SourceName);
                        });
                    }

                    // __.AddLeafString("38", 82); // Name
                    // __.AddTree("A201", ___ =>
                    // { 
                    //     ___.AddLeafVar("08", 0);
                    //     ___.AddLeafBytes("10", null);
                    // });
                });

                // Message content
                _.AddTree("1A", __ => __.AddLeafBytes("0A", PackUp(chain)));
            });
        }

        // Construct multi msg tree
        var tree = new ProtoTreeRoot();
        tree.AddTree("12", _ =>
        {
            _.AddLeafString("0A", "MultiMsg");
            _.AddTree("12", msgs);
        });

        tree.AddTree(msgs);

        return ProtoTreeRoot.Serialize(tree).GetBytes();
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

    private static void ConstructJsonChain(ProtoTreeRoot root, JsonChain chain)
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

    private static void ConstructXmlChain(ProtoTreeRoot root, XmlChain chain)
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

    private static void ConstructPlainTextChain(ProtoTreeRoot root, TextChain chain)
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

    private static void ConstructAtChain(ProtoTreeRoot root, AtChain chain)
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

    private static void ConstructRecordChain(ProtoTreeRoot root, RecordChain chain)
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

    private static void ConstructImageChain(ProtoTreeRoot root, ImageChain chain)
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

    private static void ConstructQFaceChain(ProtoTreeRoot root, QFaceChain chain)
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


    // public static MessageChain PackUp(byte[] chain)
    // {
    // }
}
