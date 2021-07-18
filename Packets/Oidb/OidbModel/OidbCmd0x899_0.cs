using System;

namespace Konata.Core.Packets.Oidb.OidbModel
{
    /// <summary>
    /// 獲取群管理列表
    /// </summary>

    public class OidbCmd0x899_0 : OidbCmd0x899
    {
        public OidbCmd0x899_0(uint groupUin)

            : base(0x00, 0x00, new ReqBody
            {
                start_uin = 0,
                identify_flag = 2,
                group_code = groupUin,
                memberlist = new MemberList
                {
                    member_uin = 0,
                    privilege = 1
                }
            })
        {

        }
    }
}
