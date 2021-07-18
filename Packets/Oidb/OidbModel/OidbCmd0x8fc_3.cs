using System;

namespace Konata.Core.Packets.Oidb.OidbModel
{
    /// <summary>
    /// 設置自身群名片
    /// </summary>

    public class OidbCmd0x8fc_3 : OidbCmd0x8fc
    {
        public OidbCmd0x8fc_3(uint groupUin, uint selfUin, string cardName)
            : base(0x03, new ReqBody
            {
                group_code = groupUin,

                rpt_mem_level_info = new MemberInfo
                {
                    uin = selfUin,
                    member_card_name = cardName,
                    // <TODO> comm_rich_card_name
                },

                msg_client_info = new ClientInfo
                {
                    implat = 109,
                    clientver = "2" // getNumberOfCameras
                }
            })
        {

        }
    }
}
