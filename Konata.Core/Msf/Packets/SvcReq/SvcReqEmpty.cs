using System;
using Konata.Msf.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.SvcReq
{
    public class SvcReqEmpty : UniPacket
    {
        public SvcReqEmpty()
            : base("PushService", "SvcReqEmpty", 0x00, 0x00, 0x00, 0x00,
                  (out Jce.Struct w) => w = new Jce.Struct
                  {
                      [0] = (Jce.String)"This is an empty request."
                  })
        {

        }
    }
}
