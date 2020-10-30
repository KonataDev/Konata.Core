using System;
using Konata.Library.IO;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Oidb
{
    public delegate void OidbPayloadWriter(ProtoTreeRoot writer);
    public delegate void OidbPayloadReader(ProtoTreeRoot reader);
    public delegate void OidbByteBufferWriter(ByteBuffer writer);

    public class OidbSSOPkg : Packet
    {
        public readonly ProtoTreeRoot root;
        public readonly int svcCmd;
        public readonly int svcType;
        public readonly int? svcResult;
        public readonly string errorMsg;
        public readonly string clientVer;

        public OidbSSOPkg(int cmd, int type, int? result, OidbPayloadWriter writer)
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
                root.AddLeafByteBuffer("22", payload.Serialize());

                root.AddLeafString("28", errorMsg);
                root.AddLeafString("30", clientVer);
            }
            PutByteBuffer(root.Serialize());
        }

        public OidbSSOPkg(int cmd, int type, int? result, OidbByteBufferWriter writer)
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
            PutByteBuffer(root.Serialize());
        }

        public OidbSSOPkg(byte[] data)
        {

        }
    }
}
