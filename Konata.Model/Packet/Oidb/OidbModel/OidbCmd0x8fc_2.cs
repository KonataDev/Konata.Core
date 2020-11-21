using System;

namespace Konata.Model.Packet.Oidb.OidbModel
{
    /// <summary>
    /// 設置群成員頭銜
    /// </summary>

    public class OidbCmd0x8fc_2 : OidbCmd0x8fc
    {
        public OidbCmd0x8fc_2(uint groupUin, uint memberUin,
            string specialTitle, uint? expiredTime = null)

            : base(0x02, new ReqBody
            {
                group_code = groupUin,
                rpt_mem_level_info = new MemberInfo
                {
                    uin = memberUin,
                    uin_name = specialTitle,
                    special_title = specialTitle,
                    special_title_expire_time = expiredTime ?? uint.MaxValue
                }
            })
        {

        }
    }
}
