using System;
using System.Collections.Generic;
using Konata.Library.IO;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : ByteBuffer
    {
        public delegate void UniPacketPayloadWriter(out Jce.Struct writer);

        public readonly Jce.Struct packagePayload;
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

        public UniPacket(string servantName, string funcName,
            byte packetType, ushort messageType, ushort requestId,
            ushort oldRespIret, UniPacketPayloadWriter writer)
        {
            packageVersion = 0x03;
            packageFuncName = funcName;
            packageServantName = servantName;
            packagePacketType = packetType;
            packageMessageType = messageType;
            packageRequestId = requestId;
            packageOldRespIret = oldRespIret;

            packagePayload = new Jce.Struct();
            {
                writer(out packagePayload);
            }

            var root = new Jce.Struct
            {
                [1] = (Jce.Number)packageVersion,
                [2] = (Jce.Number)packagePacketType,
                [3] = (Jce.Number)packageMessageType,
                [4] = (Jce.Number)packageRequestId,
                [5] = (Jce.String)packageServantName,
                [6] = (Jce.String)packageFuncName,

                [7] = (Jce.Map)new Jce.Map()
                {
                    [(Jce.String)packageFuncName] = (Jce.Struct)packagePayload,
                },

                [8] = (Jce.Number)packageTimeout,
                [9] = (Jce.Map)new Jce.Map(),
                [10] = (Jce.Map)new Jce.Map(),
            };

            PutBytes(Jce.Serialize(root));
        }

        public UniPacket(string servantName, string funcName, string servantNameV2,
            byte packetType, ushort messageType, ushort requestId,
            ushort oldRespIret, UniPacketPayloadWriter writer)
        {
            packageVersion = 0x02;
            packageFuncName = funcName;
            packageServantName = servantName;
            packagePacketType = packetType;
            packageMessageType = messageType;
            packageRequestId = requestId;
            packageOldRespIret = oldRespIret;

            packagePayload = new Jce.Struct();
            {
                writer(out packagePayload);
            }

            var root = new Jce.Struct
            {
                [1] = (Jce.Number)packageVersion,
                [2] = (Jce.Number)packagePacketType,
                [3] = (Jce.Number)packageMessageType,
                [4] = (Jce.Number)packageRequestId,
                [5] = (Jce.String)packageServantName,
                [6] = (Jce.String)packageFuncName,

                [7] = (Jce.Map)new Jce.Map()
                {
                    [(Jce.String)packageFuncName] = (Jce.Map)new Jce.Map
                    {
                        [(Jce.String)servantNameV2] = (Jce.Struct)packagePayload
                    }
                },

                [8] = (Jce.Number)packageTimeout,
                [9] = (Jce.Map)new Jce.Map(),
                [10] = (Jce.Map)new Jce.Map(),
            };

            PutBytes(Jce.Serialize(root));
        }
    }
}
