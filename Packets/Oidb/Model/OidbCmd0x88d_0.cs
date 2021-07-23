using System;

namespace Konata.Core.Packets.Oidb.Model
{
    /// <summary>
    /// 拉取群聊資料
    /// </summary>

    public class OidbCmd0x88d_0 : OidbCmd0x88d
    {
        public OidbCmd0x88d_0(uint groupUin)

            : base(0x00, new ReqBody
            {
                appid = 200000020,
                stzreqgroupinfo = new ReqGroupInfo
                {
                    group_code = groupUin,
                    stgroupinfo = new GroupInfo
                    {
                        group_class_ext = 0,
                        cmduin_join_time = 0,
                        cmduin_join_msg_seq = 0,
                        cmduin_join_real_msg_seq = 0
                    }
                }
            })
        {

        }
    }
}
