using System;
using System.Collections.Generic;
using Konata.Utils.Jce;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : JceOutputStream
    {
        public readonly UniPacketBody _packageBody;
        public readonly ushort _packageVersion;
        public readonly string _packageServantName;
        public readonly string _packageFuncName;
        public readonly byte _packagePacketType;
        public readonly ushort _packageMessageType;
        public readonly ushort _packageRequestId;
        public readonly ushort _packageOldRespIret;
        public readonly ushort _packageTimeout;
        public readonly Dictionary<string, string> _packageContext;
        public readonly Dictionary<string, string> _packageStatus;

        public UniPacket(bool useVersion3, string servantName, string funcName,
            byte packetType, ushort messageType, ushort requestId, ushort oldRespIret,
            UniPacketBody body)
        {
            _packageBody = body;
            _packageServantName = servantName;
            _packageFuncName = funcName;
            _packagePacketType = packetType;
            _packageMessageType = messageType;
            _packageRequestId = requestId;
            _packageOldRespIret = oldRespIret;
            _packageVersion = (ushort)(useVersion3 ? 3 : 2);

            Write(_packageVersion, 1);
            Write(_packagePacketType, 2);
            Write(_packageMessageType, 3);
            Write(_packageRequestId, 4);
            Write(_packageServantName, 5);
            Write(_packageFuncName, 6);
            Write((Packet)_packageBody, 7);
            Write(_packageTimeout, 8);
            Write(_packageContext, 9);
            Write(_packageStatus, 10);
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
