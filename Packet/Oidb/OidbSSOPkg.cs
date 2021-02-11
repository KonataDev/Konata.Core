using System;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;

namespace Konata.Core.Packet.Oidb
{
    public delegate void OidbPayloadWriter(ProtoTreeRoot writer);
    public delegate void OidbPayloadReader(ProtoTreeRoot reader);
    public delegate void OidbByteBufferWriter(ByteBuffer writer);

    public class OidbSSOPkg : PacketBase
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

    public abstract class OidbStruct
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
}
