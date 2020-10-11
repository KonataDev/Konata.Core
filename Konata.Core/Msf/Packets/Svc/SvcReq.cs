using System;
using System.Collections.Generic;
using Konata.Msf.Packets.Wup;

namespace Konata.Msf.Packets.Svc
{
    public class SvcReq : UniPacket
    {
        public SvcReq(string servantName, string funcName,
            byte packetType, ushort messageType, ushort requestId, ushort oldRespIret,
            SvcReqBody svcRequest)

            : base(true, servantName, funcName, packetType, messageType,
                  requestId, oldRespIret, new SvcReqBody(funcName, svcRequest))
        {

        }
    }

    public class SvcReqBody : UniPacketBody
    {
        public SvcReqBody()
            : base()
        {

        }

        public SvcReqBody(string funcName, SvcReqBody body)
            : base()
        {
            var dict = new Dictionary<string, SvcReqBody>();
            dict.Add(funcName, body);
            {
                Write(dict, 0);
            }
        }
    }
}
