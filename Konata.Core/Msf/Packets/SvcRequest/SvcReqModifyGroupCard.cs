using System;
using Konata.Msf.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.SvcRequest
{
    public class SvcReqModifyGroupCard : UniPacket
    {
        public SvcReqModifyGroupCard(uint groupUin, uint memberUin, string memberCard)

            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                  "ModifyGroupCardReq", "MGCREQ", 0x00, 0x00, 1761882478,
                  (out Jce.Struct w) => w = new Jce.Struct
                  {
                      [0] = (Jce.Number)0,
                      [1] = (Jce.Number)groupUin,
                      [2] = (Jce.Number)0,

                      [3] = (Jce.List)new Jce.List
                      {
                          [0] = (Jce.Struct)new Jce.Struct
                          {
                              [0] = (Jce.Number)memberUin,
                              [1] = (Jce.Number)1,
                              [2] = (Jce.String)memberCard,
                              [3] = (Jce.Number)255,
                              [4] = (Jce.String)"",
                              [5] = (Jce.String)"",
                              [6] = (Jce.String)"",
                          }
                      }
                  })
        {
        }
    }
}
