using System;
using System.Collections.Generic;
using Konata.Library.IO;
using Konata.Library.JceStruct;
using Konata.Utils;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : ByteBuffer
    {
        public delegate void UniPacketPayloadWriter(out Jce.Struct writer);
        public delegate void UniPacketPayloadReader(object userdata, Jce.Struct reader);

        public readonly Jce.Struct packagePayload;
        public readonly ushort packageVersion;
        public readonly string packageServantName;
        public readonly string packageFuncName;
        public readonly string packageSubFuncName;
        public readonly byte packagePacketType;
        public readonly ushort packageMessageType;
        public readonly uint packageRequestId;
        public readonly ushort packageOldRespIret;
        public readonly ushort packageTimeout;
        public readonly Dictionary<string, string> packageContext;
        public readonly Dictionary<string, string> packageStatus;

        public UniPacket(byte pktVersion, string servantName, string funcName,
            string subFuncName, byte packetType, ushort messageType, uint requestId,
            UniPacketPayloadWriter writer)
        {
            packageVersion = pktVersion;
            packageFuncName = funcName;
            packageSubFuncName = subFuncName;
            packageServantName = servantName;
            packagePacketType = packetType;
            packageMessageType = messageType;
            packageRequestId = requestId;

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

                [7] = pktVersion == 0x03

                ? (Jce.Map)new Jce.Map()
                {
                    [(Jce.String)packageSubFuncName] = (Jce.Struct)packagePayload,
                }

                : (Jce.Map)new Jce.Map()
                {
                    [(Jce.String)packageFuncName] = (Jce.Map)new Jce.Map
                    {
                        [(Jce.String)packageSubFuncName] = (Jce.Struct)packagePayload
                    }
                },

                [8] = (Jce.Number)packageTimeout,
                [9] = (Jce.Map)new Jce.Map(),
                [10] = (Jce.Map)new Jce.Map(),
            };

            PutBytes(Jce.Serialize(root));
        }

        public UniPacket(byte[] payload, UniPacketPayloadReader reader)
        {
            var root = Jce.Deserialize(payload);
            {
                packageVersion = (ushort)(Jce.Number)root["1"];
                packagePacketType = (byte)(Jce.Number)root["2"];
                packageMessageType = (ushort)(Jce.Number)root["3"];
                packageRequestId = (ushort)(Jce.Number)root["4"];
                packageServantName = (string)(Jce.String)root["5"];
                packageFuncName = (string)(Jce.String)root["6"];

                switch (packageVersion)
                {
                    case 0x02:
                        packageSubFuncName = (string)(Jce.String)root["7.0.0.1.0.0"];
                        packagePayload = (Jce.Struct)(Jce.SimpleList)root["7.0.0.1.0.1"];
                        break;
                    case 0x03:
                        packagePayload = (Jce.Struct)root["7.0.0.1"];
                        break;

                    // case 0x01:
                    default:
                        throw new Exception("Unsupported unipacket type.");
                }

                reader(this, packagePayload);
            }
        }
    }
}
