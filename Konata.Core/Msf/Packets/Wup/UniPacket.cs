using System;
using System.Collections.Generic;
using Konata.Library.IO;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : ByteBuffer
    {
        public readonly UniPacketBody packageBody;
        public readonly ushort packageVersion;
        public readonly string packageServantName;
        public readonly string packageFuncName;
        public readonly byte packagePacketType;
        public readonly ushort packageMessageType;
        public readonly ushort packageRequestId;
        public readonly ushort packageOldRespIret;
        public readonly ushort packageTimeout;
        public readonly Dictionary<string, string> packageContext;
        public readonly Dictionary<string, string> packageStatus;

        private JceTreeRoot root;

        public UniPacket(bool useVersion3, string servantName, string funcName,
            byte packetType, ushort messageType, ushort requestId, ushort oldRespIret,
            UniPacketBody body)
        {
            packageBody = body;
            packageServantName = servantName;
            packageFuncName = funcName;
            packagePacketType = packetType;
            packageMessageType = messageType;
            packageRequestId = requestId;
            packageOldRespIret = oldRespIret;
            packageVersion = (ushort)(useVersion3 ? 3 : 2);

            root = new JceTreeRoot();
            {
                root.AddLeafNumber(1, packageVersion);
                root.AddLeafNumber(2, packagePacketType);
                root.AddLeafNumber(3, packageMessageType);
                root.AddLeafNumber(4, packageRequestId);
                root.AddLeafString(5, packageServantName);
                root.AddLeafString(6, packageFuncName);
                root.AddTree(7, packageBody);
                root.AddLeafNumber(8, packageTimeout);
                root.AddLeafMap(9, packageContext);
                root.AddLeafMap(10, packageStatus);
            }
            PutByteBuffer(root.Serialize());
        }
    }

    public class UniPacketBody : JceTreeRoot
    {
        public UniPacketBody()
            : base()
        {

        }
    }
}
