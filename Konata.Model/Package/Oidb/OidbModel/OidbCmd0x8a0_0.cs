using System;

namespace Konata.Model.Package.Oidb.OidbModel
{
    /// <summary>
    /// 移除多個群成員
    /// <TODO> 支援批量移除群成員
    /// </summary>

    public class OidbCmd0x8a0_0 : OidbCmd0x8a0
    {
        public OidbCmd0x8a0_0(uint groupUin, uint[] memberUin, bool preventRequest)

            : base(0x00, new ReqBody
            {
                group_code = groupUin,
                msg_kick_list = new KickMemberInfo
                {
                    operate = 5,
                    member_uin = memberUin[0],
                    flag = preventRequest ? 1U : 0U
                }
            })
        {

        }
    }
}
