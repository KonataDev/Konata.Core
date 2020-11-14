using System;
using Konata.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Packets.SvcRequest
{
    public class SvcReqGetTroopListReqV2Simplify : UniPacket
    {
        public SvcReqGetTroopListReqV2Simplify(uint selfUin)

            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                  "GetTroopListReqV2Simplify", "GetTroopListReqV2Simplify",
                  0x00, 0x00, 117456266,
                  (out Jce.Struct w) => w = new Jce.Struct
                  {
                      [0] = (Jce.Number)selfUin,
                      [1] = (Jce.Number)0,
                      [4] = (Jce.Number)1,
                      [5] = (Jce.Number)7,
                      [6] = (Jce.Number)0,
                      [7] = (Jce.Number)1,
                      [8] = (Jce.Number)1,
                  })
        {

        }
    }
}
