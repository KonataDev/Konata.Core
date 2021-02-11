using System;
using Konata.Core.Packet.Wup;
using Konata.Utils.JceStruct.Model;
namespace Konata.Core.Packet.SvcRequest
{
    public class SvcReqModifyGroupCard : UniPacket
    {
        public SvcReqModifyGroupCard(uint groupUin, uint memberUin, string memberCard)

            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                  "ModifyGroupCardReq", "MGCREQ", 0x00, 0x00, 1761882478,
                  (out JStruct w) => w = new JStruct
                  {
                      [0] = (JNumber)0,
                      [1] = (JNumber)groupUin,
                      [2] = (JNumber)0,

                      [3] = (JList)new JList
                      {
                          [0] = (JStruct)new JStruct
                          {
                              [0] = (JNumber)memberUin,
                              [1] = (JNumber)1,
                              [2] = (JString)memberCard,
                              [3] = (JNumber)255,
                              [4] = (JString)"",
                              [5] = (JString)"",
                              [6] = (JString)"",
                          }
                      }
                  })
        {
        }
    }
}
