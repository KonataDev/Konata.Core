using System;

namespace Konata.Model.Package.Oidb.OidbModel
{
    /// <summary>
    /// 移除單個群成員
    /// </summary>
    
    public class OidbCmd0x8a0_1 : OidbCmd0x8a0
    {
        public OidbCmd0x8a0_1(uint groupUin, uint memberUin, bool preventRequest)

            : base(0x01, new ReqBody
            {
                group_code = groupUin,
                kick_list = memberUin,
                kick_flag = preventRequest ? 1U : 0U
            })
        {

        }
    }
}
