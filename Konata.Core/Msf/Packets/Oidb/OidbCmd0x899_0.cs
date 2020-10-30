using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Oidb
{
    /// <summary>
    /// 拉取群管理列表
    /// 或許可以有更大的用途
    /// </summary>

    public class OidbCmd0x899_0 : OidbSSOPkg
    {
        public OidbCmd0x899_0(uint group)

            : base(0x899, 0x00, (ProtoTreeRoot root) =>
            {
                root.addLeafVar("08", group); // uint64_group_code
                root.addLeafVar("10", 0);     // uint64_start_uin
                root.addLeafVar("18", 2);     // uint32_identify_flag

                //  8 uint64_group_code
                // 16 uint64_start_uin
                // 24 uint32_identify_flag
                // 32 rpt_uint64_uin_list
                // 42 memberlist_opt
                // 48 uint32_member_num
                // 56 uint32_filter_method
                // 64 uint32_online_flag

                root.addTree("2A", (ProtoTreeRoot memberlist) =>
                {
                    memberlist.addLeafVar("08", 0);   // uint64_member_uin
                    memberlist.addLeafVar("9001", 1); // uint32_privilege

                    //   8 uint64_member_uin
                    //  16 uint32_uin_flag
                    //  24 uint32_uin_flagex
                    //  32 uint32_uin_mobile_flag
                    //  40 uint32_uin_arch_flag
                    //  48 uint32_join_time
                    //  56 uint32_old_msg_seq
                    //  64 uint32_new_msg_seq
                    //  72 uint32_last_speak_time
                    //  80 uint32_level
                    //  88 uint32_point
                    //  96 uint32_shutup_timestap
                    // 104 uint32_flagex2
                    // 114 bytes_special_title
                    // 120 uint32_special_title_expire_time
                    // 128 uint32_active_day
                    // 138 bytes_uin_key
                    // 144 uint32_privilege
                    // 154 bytes_rich_info
                });
            })
        {

        }
    }
}
