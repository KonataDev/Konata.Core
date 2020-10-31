using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Oidb
{
    /// <summary>
    /// 獲取群内相關數據
    /// </summary>

    public class OidbCmd0x88d_0 : OidbSSOPkg
    {
        public OidbCmd0x88d_0(uint group, GroupInfo info)

            : base(0x88d, 0x00, null, (ProtoTreeRoot root) =>
            {
                // 0x08 uint32_appid
                // 0x12 stzreqgroupinfo
                // 0x18 uint32_pc_client_version

                root.AddLeafVar("08", 200000020);
                root.AddTree("12", (ProtoTreeRoot groupReq) =>
                {
                    // 0x08 uint64_group_code
                    // 0x12 stgroupinfo
                    // 0x18 uint32_last_get_group_name_time

                    groupReq.AddLeafVar("08", group);
                    groupReq.AddTree("12", (ProtoTreeRoot groupInfo) =>
                    {
                        groupInfo.AddLeafVar("08", info.group_owner);
                        groupInfo.AddLeafVar("10", info.group_create_time);
                        groupInfo.AddLeafVar("18", info.group_flag);
                        groupInfo.AddLeafVar("20", info.group_flag_ext);
                        groupInfo.AddLeafVar("28", info.group_member_max_num);
                        groupInfo.AddLeafVar("30", info.group_member_num);
                        groupInfo.AddLeafVar("38", info.group_option);
                        groupInfo.AddLeafVar("40", info.group_class_ext);
                        groupInfo.AddLeafVar("48", info.group_special_class);
                        groupInfo.AddLeafVar("50", info.group_level);
                        groupInfo.AddLeafVar("58", info.group_face);
                        groupInfo.AddLeafVar("60", info.group_default_page);
                        groupInfo.AddLeafVar("68", info.group_info_seq);
                        groupInfo.AddLeafVar("70", info.group_roaming_time);
                        groupInfo.AddLeafString("7A", info.group_name);
                        groupInfo.AddLeafString("8201", info.group_memo);
                        groupInfo.AddLeafString("8A01", info.group_finger_memo);
                        groupInfo.AddLeafString("9201", info.group_class_text);
                        groupInfo.AddLeafVar("9801", info.group_alliance_code);
                        groupInfo.AddLeafVar("A001", info.group_extra_adm_num);
                        groupInfo.AddLeafVar("A801", info.group_uin);
                        groupInfo.AddLeafVar("B001", info.group_cur_msg_seq);
                        groupInfo.AddLeafVar("B801", info.group_last_msg_time);
                        groupInfo.AddLeafString("C201", info.group_question);
                        groupInfo.AddLeafString("CA01", info.group_answer);
                        groupInfo.AddLeafVar("D001", info.group_visitor_max_num);
                        groupInfo.AddLeafVar("D801", info.group_visitor_cur_num);
                        groupInfo.AddLeafVar("E001", info.level_name_seq);
                        groupInfo.AddLeafVar("E801", info.group_admin_max_num);
                        groupInfo.AddLeafVar("F001", info.group_aio_skin_timestamp);
                        groupInfo.AddLeafVar("F801", info.group_board_skin_timestamp);
                        groupInfo.AddLeafString("8202", info.group_aio_skin_url);
                        groupInfo.AddLeafString("8A02", info.group_board_skin_url);
                        groupInfo.AddLeafVar("9002", info.group_cover_skin_timestamp);
                        groupInfo.AddLeafString("9A02", info.group_cover_skin_url);
                        groupInfo.AddLeafVar("A002", info.group_grade);
                        groupInfo.AddLeafVar("A802", info.active_member_num);
                        groupInfo.AddLeafVar("B002", info.certification_type);
                        groupInfo.AddLeafString("BA02", info.certification_text);
                        groupInfo.AddLeafString("C202", info.group_rich_finger_memo);
                        //  groupInfo.AddLeafVar(330, rpt_tag_record);
                        //  groupInfo.AddLeafVar(338, group_geo_info);
                        groupInfo.AddLeafVar("D802", info.head_portrait_seq);
                        // groupInfo.AddLeafVar(354, msg_head_portrait);
                        groupInfo.AddLeafVar("E802", info.shutup_timestamp);
                        groupInfo.AddLeafVar("F002", info.shutup_timestamp_me);
                        groupInfo.AddLeafVar("F802", info.create_source_flag);
                        groupInfo.AddLeafVar("8003", info.cmduin_msg_seq);
                        groupInfo.AddLeafVar("8803", info.cmduin_join_time);
                        groupInfo.AddLeafVar("9003", info.cmduin_uin_flag);
                        groupInfo.AddLeafVar("9803", info.cmduin_flag_ex);
                        groupInfo.AddLeafVar("A003", info.cmduin_new_mobile_flag);
                        groupInfo.AddLeafVar("A803", info.cmduin_read_msg_seq);
                        groupInfo.AddLeafVar("B003", info.cmduin_last_msg_time);
                        groupInfo.AddLeafVar("B803", info.group_type_flag);
                        groupInfo.AddLeafVar("C003", info.app_privilege_flag);
                        // groupInfo.AddLeafVar(458 st_group_ex_info);
                        groupInfo.AddLeafVar("D003", info.group_sec_level);
                        groupInfo.AddLeafVar("D803", info.group_sec_level_info);
                        groupInfo.AddLeafVar("E003", info.cmduin_privilege);
                        // groupInfo.AddLeafString(490, info.poid_info);
                        groupInfo.AddLeafVar("F003", info.cmduin_flag_ex2);
                        groupInfo.AddLeafVar("F803", info.conf_uin);
                        groupInfo.AddLeafVar("8004", info.conf_max_msg_seq);
                        groupInfo.AddLeafVar("8804", info.conf_to_group_time);
                        groupInfo.AddLeafVar("9004", info.password_redbag_time);
                        groupInfo.AddLeafVar("9804", info.subscription_uin);
                        groupInfo.AddLeafVar("A004", info.member_list_change_seq);
                        groupInfo.AddLeafVar("A804", info.membercard_seq);
                        groupInfo.AddLeafVar("B004", info.root_id);
                        groupInfo.AddLeafVar("B804", info.parent_id);
                        groupInfo.AddLeafVar("C004", info.team_seq);
                        groupInfo.AddLeafVar("C804", info.history_msg_begin_time);
                        groupInfo.AddLeafVar("D004", info.invite_no_auth_num_limit);
                        groupInfo.AddLeafVar("D804", info.cmduin_history_msg_seq);
                        groupInfo.AddLeafVar("E004", info.cmduin_join_msg_seq);
                        groupInfo.AddLeafVar("E804", info.group_flagext3);
                        groupInfo.AddLeafVar("F004", info.group_open_appid);
                        groupInfo.AddLeafVar("F804", info.is_conf_group);
                        groupInfo.AddLeafVar("8005", info.is_modify_conf_group_face);
                        groupInfo.AddLeafVar("8805", info.is_modify_conf_group_name);
                        groupInfo.AddLeafVar("9005", info.no_finger_open_flag);
                        groupInfo.AddLeafVar("9805", info.no_code_finger_open_flag);
                        groupInfo.AddLeafVar("A005", info.auto_agree_join_group_user_num_for_normal_group);
                        groupInfo.AddLeafVar("A805", info.auto_agree_join_group_user_num_for_conf_group);
                        groupInfo.AddLeafVar("B005", info.is_allow_conf_group_member_nick);
                        groupInfo.AddLeafVar("B805", info.is_allow_conf_group_member_at_all);
                        groupInfo.AddLeafVar("C005", info.is_allow_conf_group_member_modify_group_name);
                        groupInfo.AddLeafString("CA05", info.long_group_name);
                        groupInfo.AddLeafVar("D005", info.cmduin_join_real_msg_seq);
                        groupInfo.AddLeafVar("D805", info.is_group_freeze);
                        groupInfo.AddLeafVar("E005", info.msg_limit_frequency);
                        groupInfo.AddLeafBytes("EA05", info.join_group_auth);
                        groupInfo.AddLeafVar("F005", info.hl_guild_appid);
                        groupInfo.AddLeafVar("F805", info.hl_guild_sub_type);
                        groupInfo.AddLeafVar("8006", info.hl_guild_orgid);
                        groupInfo.AddLeafVar("8806", info.is_allow_hl_guild_binary);
                        groupInfo.AddLeafVar("9006", info.cmduin_ringtone_id);
                        groupInfo.AddLeafVar("9806", info.group_flagext4);
                        groupInfo.AddLeafVar("A006", info.group_freeze_reason);
                        groupInfo.AddLeafVar("A806", info.is_allow_recall_msg);

                    });
                    groupReq.AddLeafVar("18", null);
                });
                root.AddLeafVar("18", null);
            })
        {

        }

        public struct GroupInfo
        {
            // 0x08
            public long? group_owner;

            // 0x10
            public uint? group_create_time;

            // 0x18
            public uint? group_flag;

            // 0x20
            public uint? group_flag_ext;

            // 0x28
            public uint? group_member_max_num;

            // 0x30
            public uint? group_member_num;

            // 0x38
            public uint? group_option;

            // 0x40
            public uint? group_class_ext;

            // 0x48
            public uint? group_special_class;

            // 0x50
            public uint? group_level;

            // 0x58
            public uint? group_face;

            // 0x60
            public uint? group_default_page;

            // 0x68
            public uint? group_info_seq;

            // 0x70
            public uint? group_roaming_time;

            // 0x7A
            public string group_name;

            // 0x8201
            public string group_memo;

            // 0x8A01
            public string group_finger_memo;

            // 0x9201
            public string group_class_text;

            // 0x9801
            public uint? group_alliance_code;

            // 0xA001
            public uint? group_extra_adm_num;

            // 0xA801
            public long? group_uin;

            // 0xB001
            public uint? group_cur_msg_seq;

            // 0xB801
            public uint? group_last_msg_time;

            // 0xC201
            public string group_question;

            // 0xCA01
            public string group_answer;

            // 0xD001
            public uint? group_visitor_max_num;

            // 0xD801
            public uint? group_visitor_cur_num;

            // 0xE001
            public uint? level_name_seq;

            // 0xE801
            public uint? group_admin_max_num;

            // 0xF001
            public uint? group_aio_skin_timestamp;

            // 0xF801
            public uint? group_board_skin_timestamp;

            // 0x8202
            public string group_aio_skin_url;

            // 0x8A02
            public string group_board_skin_url;

            // 0x9002
            public uint? group_cover_skin_timestamp;

            // 0x9A02
            public string group_cover_skin_url;

            // 0xA002
            public uint? group_grade;

            // 0xA802
            public uint? active_member_num;

            // 0xB002
            public uint? certification_type;

            // 0xBA02
            public string certification_text;

            // 0xC202
            public string group_rich_finger_memo;

            // 0xCA02 rpt_tag_record

            // 0xD202 group_geo_info

            // 0xD802
            public uint? head_portrait_seq;

            // 0xE202 msg_head_portrait

            // 0xE802
            public uint? shutup_timestamp;

            // 0xF002
            public uint? shutup_timestamp_me;

            // 0xF802
            public uint? create_source_flag;

            // 0x8003
            public uint? cmduin_msg_seq;

            // 0x8803
            public uint? cmduin_join_time;

            // 0x9003
            public uint? cmduin_uin_flag;

            // 0x9803
            public uint? cmduin_flag_ex;

            // 0xA003
            public uint? cmduin_new_mobile_flag;

            // 0xA803
            public uint? cmduin_read_msg_seq;

            // 0xB003
            public uint? cmduin_last_msg_time;

            // 0xB803
            public uint? group_type_flag;

            // 0xC003
            public uint? app_privilege_flag;

            // 0xCA03 st_group_ex_info

            // 0xD003
            public uint? group_sec_level;

            // 0xD803
            public uint? group_sec_level_info;

            // 0xE003
            public uint? cmduin_privilege;

            // 0xEA03
            public string poid_info;

            // 0xF003
            public uint? cmduin_flag_ex2;

            // 0xF803
            public long? conf_uin;

            // 0x8004
            public uint? conf_max_msg_seq;

            // 0x8804
            public uint? conf_to_group_time;

            // 0x9004
            public uint? password_redbag_time;

            // 0x9804
            public long? subscription_uin;

            // 0xA004
            public uint? member_list_change_seq;

            // 0xA804
            public uint? membercard_seq;

            // 0xB004
            public long? root_id;

            // 0xB804
            public long? parent_id;

            // 0xC004
            public uint? team_seq;

            // 0xC804
            public long? history_msg_begin_time;

            // 0xD004
            public long? invite_no_auth_num_limit;

            // 0xD804
            public uint? cmduin_history_msg_seq;

            // 0xE004
            public uint? cmduin_join_msg_seq;

            // 0xE804
            public uint? group_flagext3;

            // 0xF004
            public uint? group_open_appid;

            // 0xF804
            public uint? is_conf_group;

            // 0x8005
            public uint? is_modify_conf_group_face;

            // 0x8805
            public uint? is_modify_conf_group_name;

            // 0x9005
            public uint? no_finger_open_flag;

            // 0x9805
            public uint? no_code_finger_open_flag;

            // 0xA005
            public uint? auto_agree_join_group_user_num_for_normal_group;

            // 0xA805
            public uint? auto_agree_join_group_user_num_for_conf_group;

            // 0xB005
            public uint? is_allow_conf_group_member_nick;

            // 0xB805
            public uint? is_allow_conf_group_member_at_all;

            // 0xC005
            public uint? is_allow_conf_group_member_modify_group_name;

            // 0xCA05
            public string long_group_name;

            // 0xD005
            public uint? cmduin_join_real_msg_seq;

            // 0xD805
            public uint? is_group_freeze;

            // 0xE005
            public uint? msg_limit_frequency;

            // 0xEA05
            public byte[] join_group_auth;

            // 0xF005
            public uint? hl_guild_appid;

            // 0xF805
            public uint? hl_guild_sub_type;

            // 0x8006
            public uint? hl_guild_orgid;

            // 0x8806
            public uint? is_allow_hl_guild_binary;

            // 0x9006
            public uint? cmduin_ringtone_id;

            // 0x9806
            public uint? group_flagext4;

            // 0xA006
            public uint? group_freeze_reason;

            // 0xA806
            public uint? is_allow_recall_msg;

        }

        public struct TagRecord
        {
            // 0x08
            public long? from_uin;

            // 0x10
            public long? group_code;

            // 0x1A
            public byte[] tag_id;

            // 0x20
            public long? set_time;

            // 0x28
            public uint? good_num;

            // 0x30
            public uint? bad_num;

            // 0x38
            public uint? tag_len;

            // 0x42
            public byte[] tag_value;
        }

        public struct GroupGeoInfo
        {
            // 0x08
            public long? owneruin;

            // 0x10
            public uint? settime;

            // 0x18
            public uint? cityid;

            // 0x20
            public long? longitude;

            // 0x28
            public long? latitude;

            // 0x32
            public byte[] geocontent;

            // 0x38
            public long? poi_id;
        }

        public struct GroupHeadPortrait
        {
            // 0x08
            public uint? pic_cnt;

            // 0x12 rpt_msg_info

            // 0x18
            public uint? default_id;

            // 0x20
            public uint? verifying_pic_cnt;

            // 0x2A rpt_msg_verifyingpic_info
        }

        public struct GroupHeadPortraitInfo
        {
            // 0x08 rpt_uint32_pic_id

            // 0x10
            public uint? left_x;

            // 0x18
            public uint? left_y;

            // 0x20
            public uint? right_x;

            // 0x28
            public uint? right_y;
        }
    }
}
