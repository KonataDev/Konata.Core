using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcRequest
{
    public class SvcReqGetTroopMemberListReq : UniPacket
    {
        public SvcReqGetTroopMemberListReq(uint selfUin, uint groupUin, ulong groupCode, uint nextUin)
            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                "GetTroopMemberListReq", "GTML", 0x00, 0x00, 117456266,
                (out JStruct w) => w = new JStruct
                {
                    [0] = (JNumber) selfUin,
                    [1] = (JNumber) groupUin ,
                    [2] = (JNumber) nextUin,
                    [3] = (JNumber)(long) groupCode,
                    [4] = (JNumber) 2, // Version
                    [5] = (JNumber) 2, // ReqType
                    [6] = (JNumber) 0, // GetListAppointTime
                    [7] = (JNumber) 1, // RichCardNameVer
                })
        {
        }
    }
}
