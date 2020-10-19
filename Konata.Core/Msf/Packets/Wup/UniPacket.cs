using System;
using System.Collections.Generic;
using Konata.Library.IO;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : JceOutputStream
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

            Write(packageVersion, 1);
            Write(packagePacketType, 2);
            Write(packageMessageType, 3);
            Write(packageRequestId, 4);
            Write(packageServantName, 5);
            Write(packageFuncName, 6);
            Write((ByteBuffer)packageBody, 7);
            Write(packageTimeout, 8);
            Write(packageContext, 9);
            Write(packageStatus, 10);
        }
    }

    public class UniPacketBody : JceOutputStream
    {
        public UniPacketBody()
            : base()
        {

        }
    }
}
