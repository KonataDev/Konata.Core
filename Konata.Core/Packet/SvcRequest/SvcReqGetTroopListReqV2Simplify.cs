using System;
using Konata.Model.Packet.Wup;
using Konata.Utils.JceStruct.Model;

namespace Konata.Model.Packet.SvcRequest
{
    public class SvcReqGetTroopListReqV2Simplify : UniPacket
    {
        public SvcReqGetTroopListReqV2Simplify(uint selfUin)

            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                  "GetTroopListReqV2Simplify", "GetTroopListReqV2Simplify",
                  0x00, 0x00, 117456266,
                  (out JStruct w) => w = new JStruct
                  {
                      [0] = (JNumber)selfUin,
                      [1] = (JNumber)0,
                      [4] = (JNumber)1,
                      [5] = (JNumber)7,
                      [6] = (JNumber)0,
                      [7] = (JNumber)1,
                      [8] = (JNumber)1,
                  })
        {

        }
    }
}
