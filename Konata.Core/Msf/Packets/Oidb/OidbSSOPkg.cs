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
        public readonly int svcResult;
        public readonly string errorMsg;
        public readonly string clientVer;

        public OidbSSOPkg(int cmd, int type, OidbPayloadWriter writer)
        {
            svcCmd = cmd;
            svcType = type;

            root = new ProtoTreeRoot();
            {
                root.addLeafVar("08", svcCmd);
                root.addLeafVar("10", svcType);
                root.addLeafVar("18", svcResult);

                var payload = new ProtoTreeRoot();
                {
                    writer(payload);
                }
                root.addLeafByteBuffer("22", payload.Serialize());

                root.addLeafString("28", errorMsg);
                root.addLeafString("30", clientVer);
            }
            PutByteBuffer(root.Serialize());
        }

        public OidbSSOPkg(int cmd, int type, OidbByteBufferWriter writer)
        {
            svcCmd = cmd;
            svcType = type;

            root = new ProtoTreeRoot();
            {
                root.addLeafVar("08", svcCmd);
                root.addLeafVar("10", svcType);
                root.addLeafVar("18", svcResult);

                var payload = new ByteBuffer();
                {
                    writer(payload);
                }
                root.addLeafByteBuffer("22", payload);

                root.addLeafString("28", errorMsg);
                root.addLeafString("30", clientVer);
            }
            PutByteBuffer(root.Serialize());
        }

        public OidbSSOPkg(byte[] data)
        {

        }
    }
}
