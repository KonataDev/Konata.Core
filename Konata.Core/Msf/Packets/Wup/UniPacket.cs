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

        public UniPacket(string servantName, string funcName,
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
            packageVersion = (ushort)((body is UniPacketBodyV2) ? 2 : 3);

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

        public UniPacket(byte[] data)
        {
            root = new JceTreeRoot(data);
            {
                root.GetLeafNumber(1, out packageVersion);
                root.GetLeafNumber(2, out packagePacketType);
                root.GetLeafNumber(3, out packageMessageType);
                root.GetLeafNumber(4, out packageRequestId);
                root.GetLeafString(5, out packageServantName);
                root.GetLeafString(6, out packageFuncName);
                root.GetTree(7, out var body); packageBody = (UniPacketBody)body;
                root.GetLeafNumber(8, out packageTimeout);
            }
        }
    }

    public class UniPacketBody : JceTreeRoot
    {
        public UniPacketBody()
            : base()
        {

        }
    }

    public class UniPacketBodyV2 : UniPacketBody
    {
        public UniPacketBodyV2(string reqName, string funcName, JceTreeRoot body)
            : base()
        {
            AddLeafMap(0, new Dictionary<string, JceTreeRoot>
            {
                [reqName] = new Func<JceTreeRoot>(() =>
                {
                    var req = new JceTreeRoot();
                    req.AddLeafMap(1, new Dictionary<string, JceTreeRoot>
                    {
                        [funcName] = body
                    });
                    return req;
                })()
            });
        }
    }

    public class UniPacketBodyV3 : UniPacketBody
    {
        public UniPacketBodyV3(string funcName, JceTreeRoot body)
            : base()
        {
            AddLeafMap(0, new Dictionary<string, JceTreeRoot>
            {
                [funcName] = body
            });
        }
    }
}
