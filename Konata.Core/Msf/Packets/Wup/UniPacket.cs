using System;
using System.Collections.Generic;
using System.Linq;
using Konata.Library.IO;
using Konata.Library.JceStruct;
using Konata.Utils;

namespace Konata.Msf.Packets.Wup
{
    public class UniPacket : ByteBuffer
    {
        //public readonly UniPacketBody packageBody;
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

        //private JceTreeRoot root;

        //public UniPacket(string servantName, string funcName,
        //    byte packetType, ushort messageType, ushort requestId, ushort oldRespIret,
        //    UniPacketBody body)
        //{
        //    packageBody = body;
        //    packageServantName = servantName;
        //    packageFuncName = funcName;
        //    packagePacketType = packetType;
        //    packageMessageType = messageType;
        //    packageRequestId = requestId;
        //    packageOldRespIret = oldRespIret;
        //    packageVersion = (ushort)((body is UniPacketBodyV2) ? 2 : 3);

        //    root = new JceTreeRoot();
        //    {
        //        root.AddLeafNumber(1, packageVersion);
        //        root.AddLeafNumber(2, packagePacketType);
        //        root.AddLeafNumber(3, packageMessageType);
        //        root.AddLeafNumber(4, packageRequestId);
        //        root.AddLeafString(5, packageServantName);
        //        root.AddLeafString(6, packageFuncName);
        //        root.AddTree(7, packageBody);
        //        root.AddLeafNumber(8, packageTimeout);
        //        root.AddLeafMap(9, packageContext);
        //        root.AddLeafMap(10, packageStatus);
        //    }
        //    PutByteBuffer(root.Serialize());
        //}

        //public UniPacket(byte[] data)
        //{
        //    root = new JceTreeRoot(data);
        //    {
        //        root.GetLeafNumber(1, out packageVersion);
        //        root.GetLeafNumber(2, out packagePacketType);
        //        root.GetLeafNumber(3, out packageMessageType);
        //        root.GetLeafNumber(4, out packageRequestId);
        //        root.GetLeafString(5, out packageServantName);
        //        root.GetLeafString(6, out packageFuncName);

        //        root.GetTree(7, out byte[] body);
        //        packageBody = (packageVersion == 2) ?
        //            (UniPacketBody)new UniPacketBodyV2(body) : new UniPacketBodyV3(body);

        //        root.GetLeafNumber(8, out packageTimeout);
        //    }
        //}
    }

    //public class UniPacketBody : JceTreeRoot
    //{
    //    public UniPacketBody()
    //        : base()
    //    {

    //    }

    //    public UniPacketBody(byte[] data)
    //        : base(data)
    //    {

    //    }
    //}

    //public class UniPacketBodyV2 : UniPacketBody
    //{
    //    public readonly JceTreeRoot payload;

    //    public UniPacketBodyV2(string reqName, string funcName, JceTreeRoot data)
    //        : base()
    //    {
    //        AddLeafMap(0, new Dictionary<string, JceTreeRoot>
    //        {
    //            [reqName] = new Func<JceTreeRoot>(() =>
    //            {
    //                return new JceTreeRoot()
    //                .AddLeafMap(1, new Dictionary<string, JceTreeRoot>
    //                {
    //                    [funcName] = data
    //                });
    //            })()
    //        });
    //    }

    //    public UniPacketBodyV2(byte[] data)
    //        : base(data)
    //    {
    //        Console.WriteLine(Hex.Bytes2HexStr(data));
    //        var k = GetLeafMap<string, JceTreeRoot>(0)
    //            .FirstOrDefault().Key;
    //    }
    //}

    //public class UniPacketBodyV3 : UniPacketBody
    //{
    //    public readonly JceTreeRoot payload;

    //    public UniPacketBodyV3(string funcName, JceTreeRoot data)
    //        : base()
    //    {
    //        payload = data;

    //        AddLeafMap(0, new Dictionary<string, JceTreeRoot>
    //        {
    //            [funcName] = data
    //        });
    //    }

    //    public UniPacketBodyV3(byte[] data)
    //        : base(data)
    //    {
    //        payload = GetLeafMap<string, JceTreeRoot>(0)
    //            .FirstOrDefault().Value;
    //    }
    //}
}
