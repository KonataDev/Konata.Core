using System;
using System.Collections.Generic;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.Wup
{
    public class UniPacket : ByteBuffer
    {
        public delegate void UniPacketPayloadWriter(out JStruct writer);
        public delegate void UniPacketPayloadReader(object userdata, JStruct reader);

        public readonly JStruct packagePayload;
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

            packagePayload = new JStruct();
            {
                writer(out packagePayload);
            }

            var root = new JStruct
            {
                [1] = (JNumber)packageVersion,
                [2] = (JNumber)packagePacketType,
                [3] = (JNumber)packageMessageType,
                [4] = (JNumber)packageRequestId,
                [5] = (JString)packageServantName,
                [6] = (JString)packageFuncName,

                [7] = pktVersion == 0x03 ?

                new JMap()
                {
                    [(JString)packageSubFuncName] = packagePayload.Serialize(),
                }.Serialize() :

                new JMap()
                {
                    [(JString)packageFuncName] = (JMap)new JMap
                    {
                        [(JString)packageSubFuncName] = packagePayload.Serialize()
                    }
                }.Serialize(),

                [8] = (JNumber)packageTimeout,
                [9] = (JMap)new JMap(),
                [10] = (JMap)new JMap(),
            };

            PutBytes(Jce.Serialize(root));
        }

        public UniPacket(byte[] payload, UniPacketPayloadReader reader)
        {
            var root = Jce.Deserialize(payload);
            {
                packageVersion = (ushort)root["1"].Number.Value;
                packagePacketType = (byte)root["2"].Number.Value;
                packageMessageType = (ushort)root["3"].Number.Value;
                packageRequestId = (ushort)root["4"].Number.Value;
                packageServantName = root["5"].String.Value;
                packageFuncName = root["6"].String.Value;

                switch (packageVersion)
                {
                    case 0x02:
                        packageSubFuncName = root["7.0.0.1.0.0"].String.Value;
                        packagePayload = root["7.0.0.1.0.1"].Struct;
                        break;
                    case 0x03:
                        packagePayload = root["7.0.0.1"].Struct;
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
