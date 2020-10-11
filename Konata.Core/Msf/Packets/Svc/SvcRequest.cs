using System;
using System.Collections.Generic;
using Konata.Msf.Packets.Wup;

namespace Konata.Msf.Packets.Svc
{
    public class SvcRequest : UniPacket
    {
        public SvcRequest(string servantName, string funcName,
            byte packetType, ushort messageType, ushort requestId, ushort oldRespIret,
            SvcRequestBody svcRequest)

            : base(true, servantName, funcName, packetType, messageType,
                  requestId, oldRespIret, new SvcRequestBody(funcName, svcRequest))
        {

        }
    }

    public class SvcRequestBody : UniPacketBody
    {
        public SvcRequestBody()
            : base()
        {

        }

        public SvcRequestBody(string funcName, SvcRequestBody body)
            : base()
        {
            var dict = new Dictionary<string, SvcRequestBody>();
            dict.Add(funcName, body);
            {
                Write(dict, 0);
            }
        }
    }
}
