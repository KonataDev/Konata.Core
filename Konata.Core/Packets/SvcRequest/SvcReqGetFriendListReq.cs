using System.Collections.Generic;
using Konata.Core.Packets.Wup;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcRequest
{
    public class SvcReqGetFriendListReq : UniPacket
    {
        public SvcReqGetFriendListReq(uint selfUin, uint startIndex, uint limitNum)
            : base(0x03, "mqq.IMService.FriendListServiceServantObj",
                "GetFriendListReq", "FL", 0x00, 0x00, 117456266,
                (out JStruct w) => w = new JStruct
                {
                    [0] = (JNumber) 3, // ReqType
                    [1] = (JNumber) 1, // IfReflush
                    [2] = (JNumber) selfUin, // SelfUin
                    [3] = (JNumber) startIndex, // StartIndex
                    [4] = (JNumber) limitNum, // GetFriendCount
                    [5] = (JNumber) 0, // GroupId
                    [6] = (JNumber) 0, // IfGetGroupInfo
                    [7] = (JNumber) 0, // GroupStartIndex
                    [8] = (JNumber) 0, // GetGroupCount
                    [9] = (JNumber) 0, // IfGetMSFGroup
                    [10] = (JNumber) 1, // IsShowTermType
                    [11] = (JNumber) 31, // Version
                    [13] = (JNumber) 0, // eAppType
                    [14] = (JNumber) 0, // IfGetDOVId
                    [15] = (JNumber) 0, // IfGetBothFlag

                    [16] = new JSimpleList(ProtoTreeRoot // 0xd50Req
                        .Serialize(new OidbCmd0xd50.ReqBody
                        {
                            appid = 10002,
                            req_music_switch = 1,
                            req_ksing_switch = 1,
                            req_mutualmark_lbsshare = 1,
                            req_mutualmark_alienation = 1
                        }.BuildTree()).GetBytes()),

                    [18] = new JList(new List<IObject> // SnsTypelist
                        {new JNumber(13580), new JNumber(13581), new JNumber(13582)}),
                })
        {
        }
    }
}
