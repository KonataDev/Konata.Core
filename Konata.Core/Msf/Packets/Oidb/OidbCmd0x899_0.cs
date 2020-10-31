using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Oidb
{
    /// <summary>
    /// 獲取群成員相關數據
    /// </summary>

    public class OidbCmd0x899_0 : OidbSSOPkg
    {
        public OidbCmd0x899_0(uint groupUin, uint startUin, uint identFlag,
            MemberList memberList, uint? memberNum = null, uint? filerMethod = null)

            : base(0x899, 0x00, 0x00, (ProtoTreeRoot root) =>
            {
                // 0x08 uint64_group_code
                // 0x10 uint64_start_uin
                // 0x18 uint32_identify_flag
                // 0x20 rpt_uint64_uin_list
                // 0x2A memberlist_opt
                // 0x30 uint32_member_num
                // 0x38 uint32_filter_method
                // 0x40 uint32_online_flag

                switch (identFlag)
                {
                    case 1:
                        break;
                    case 2:
                        memberList.privilege = 1;
                        break;
                    case 3:
                        break;
                    case 4:
                        memberList.uin_flag = 0;
                        break;
                    case 5:
                        root.AddLeafVar("30", memberNum);
                        root.AddLeafVar("38", filerMethod);
                        break;
                    case 6:
                        break;
                }

                root.AddLeafVar("08", groupUin);
                root.AddLeafVar("10", startUin);
                root.AddLeafVar("18", identFlag);
                root.AddLeafVar("30", memberNum);
                root.AddLeafVar("38", filerMethod);
                root.AddLeafVar("40", 0);
                root.AddTree("2A", (ProtoTreeRoot list) =>
                {
                    // 0x08 uint64_member_uin
                    // 0x10 uint32_uin_flag
                    // 0x18 uint32_uin_flagex
                    // 0x20 uint32_uin_mobile_flag
                    // 0x28 uint32_uin_arch_flag
                    // 0x30 uint32_join_time
                    // 0x38 uint32_old_msg_seq
                    // 0x40 uint32_new_msg_seq
                    // 0x48 uint32_last_speak_time
                    // 0x50 uint32_level
                    // 0x58 uint32_point
                    // 0x60 uint32_shutup_timestap
                    // 0x68 uint32_flagex2
                    // 0x72 bytes_special_title
                    // 0x78 uint32_special_title_expire_time
                    // 0x8001 uint32_active_day
                    // 0x8A01 bytes_uin_key
                    // 0x9001 uint32_privilege
                    // 0x9A01 bytes_rich_info

                    list.AddLeafVar("08", memberList.member_uin);
                    list.AddLeafVar("10", memberList.uin_flag);
                    list.AddLeafVar("18", memberList.uin_flagex);
                    list.AddLeafVar("20", memberList.uin_mobile_flag);
                    list.AddLeafVar("28", memberList.uin_arch_flag);
                    list.AddLeafVar("30", memberList.join_time);
                    list.AddLeafVar("38", memberList.old_msg_seq);
                    list.AddLeafVar("40", memberList.new_msg_seq);
                    list.AddLeafVar("48", memberList.last_speak_time);
                    list.AddLeafVar("50", memberList.level);
                    list.AddLeafVar("58", memberList.point);
                    list.AddLeafVar("60", memberList.shutup_timestap);
                    list.AddLeafVar("68", memberList.flagex2);
                    list.AddLeafBytes("72", memberList.special_title);
                    list.AddLeafVar("78", memberList.special_title_expire_time);
                    list.AddLeafVar("8001", memberList.active_day);
                    list.AddLeafBytes("8A01", memberList.uin_key);
                    list.AddLeafVar("9001", memberList.privilege);
                    list.AddLeafBytes("9A01", memberList.rich_info);
                });
            })
        {

        }

        public struct MemberList
        {
            public long? member_uin;      // 0x08
            public uint? uin_flag;        // 0x10
            public uint? uin_flagex;      // 0x18
            public uint? uin_mobile_flag; // 0x20
            public uint? uin_arch_flag;   // 0x28
            public uint? join_time;       // 0x30
            public uint? old_msg_seq;     // 0x38
            public uint? new_msg_seq;     // 0x40
            public uint? last_speak_time; // 0x48
            public uint? level;           // 0x50
            public uint? point;           // 0x58
            public uint? shutup_timestap; // 0x60
            public uint? flagex2;         // 0x68
            public byte[] special_title;  // 0x72
            public uint? special_title_expire_time;  // 0x78
            public uint? active_day;      // 0x8001
            public byte[] uin_key;        // 0x8A01
            public uint? privilege;       // 0x9001
            public byte[] rich_info;      // 0x9A01
        }
    }
}
