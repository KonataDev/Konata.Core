﻿using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Oidb;

internal delegate void OidbPayloadWriter(ProtoTreeRoot writer);

internal delegate void OidbPayloadReader(ProtoTreeRoot reader);

internal delegate void OidbByteBufferWriter(ByteBuffer writer);

internal class OidbSSOPkg : PacketBase
{
    public readonly ProtoTreeRoot root;
    public readonly uint svcCmd;
    public readonly uint svcType;
    public readonly uint? svcResult;
    public readonly string errorMsg;
    public readonly string clientVer;

    public OidbSSOPkg(uint cmd, uint type, uint? result, OidbPayloadWriter writer)
    {
        svcCmd = cmd;
        svcType = type;
        svcResult = result;

        root = new ProtoTreeRoot();
        {
            root.AddLeafVar("08", svcCmd);
            root.AddLeafVar("10", svcType);
            root.AddLeafVar("18", svcResult);

            var payload = new ProtoTreeRoot();
            {
                writer(payload);
            }
            root.AddLeafByteBuffer("22", ProtoTreeRoot.Serialize(payload));

            root.AddLeafString("28", errorMsg);
            root.AddLeafString("30", clientVer);
        }
        PutByteBuffer(ProtoTreeRoot.Serialize(root));
    }

    public OidbSSOPkg(uint cmd, uint type, uint? result, OidbByteBufferWriter writer)
    {
        svcCmd = cmd;
        svcType = type;
        svcResult = result;

        root = new ProtoTreeRoot();
        {
            root.AddLeafVar("08", svcCmd);
            root.AddLeafVar("10", svcType);
            root.AddLeafVar("18", svcResult);

            var payload = new ByteBuffer();
            {
                writer(payload);
            }
            root.AddLeafByteBuffer("22", payload);

            root.AddLeafString("28", errorMsg);
            root.AddLeafString("30", clientVer);
        }
        PutByteBuffer(ProtoTreeRoot.Serialize(root));
    }

    public OidbSSOPkg(byte[] data)
    {
    }
}

internal abstract class OidbStruct
{
    public abstract void Write(ProtoTreeRoot root);

    public ProtoTreeRoot BuildTree()
    {
        var tree = new ProtoTreeRoot();
        {
            Write(tree);
        }
        return tree;
    }
}
