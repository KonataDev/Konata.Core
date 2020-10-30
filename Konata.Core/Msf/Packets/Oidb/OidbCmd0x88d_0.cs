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
                //  8 uint32_appid
                // 18 stzreqgroupinfo
                // 24 uint32_pc_client_version

                root.AddLeafVar("08", 200000020);
                root.AddTree("12", (ProtoTreeRoot groupReq) =>
                {
                    //  8 uint64_group_code 
                    // 18 stgroupinfo
                    // 24 uint32_last_get_group_name_time

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
            public long? group_owner;
            public uint? group_create_time;
            public uint? group_flag;
            public uint? group_flag_ext;
            public uint? group_member_max_num;
            public uint? group_member_num;
            public uint? group_option;
            public uint? group_class_ext;
            public uint? group_special_class;
            public uint? group_level;
            public uint? group_face;
            public uint? group_default_page;
            public uint? group_info_seq;
            public uint? group_roaming_time;
            public string group_name;
            public string group_memo;
            public string group_finger_memo;
            public string group_class_text;
            public uint? group_alliance_code;
            public uint? group_extra_adm_num;
            public long? group_uin;
            public uint? group_cur_msg_seq;
            public uint? group_last_msg_time;
            public string group_question;
            public string group_answer;
            public uint? group_visitor_max_num;
            public uint? group_visitor_cur_num;
            public uint? level_name_seq;
            public uint? group_admin_max_num;
            public uint? group_aio_skin_timestamp;
            public uint? group_board_skin_timestamp;
            public string group_aio_skin_url;
            public string group_board_skin_url;
            public uint? group_cover_skin_timestamp;
            public string group_cover_skin_url;
            public uint? group_grade;
            public uint? active_member_num;
            public uint? certification_type;
            public string certification_text;
            public string group_rich_finger_memo;
            // public TagRecord rpt_tag_record;
            // public GroupGeoInfo group_geo_info;
            public uint? head_portrait_seq;
            // public msg_head_portrait;
            public uint? shutup_timestamp;
            public uint? shutup_timestamp_me;
            public uint? create_source_flag;
            public uint? cmduin_msg_seq;
            public uint? cmduin_join_time;
            public uint? cmduin_uin_flag;
            public uint? cmduin_flag_ex;
            public uint? cmduin_new_mobile_flag;
            public uint? cmduin_read_msg_seq;
            public uint? cmduin_last_msg_time;
            public uint? group_type_flag;
            public uint? app_privilege_flag;
            // public st_group_ex_info;
            public uint? group_sec_level;
            public uint? group_sec_level_info;
            public uint? cmduin_privilege;
            public string poid_info;
            public uint? cmduin_flag_ex2;
            public long? conf_uin;
            public uint? conf_max_msg_seq;
            public uint? conf_to_group_time;
            public uint? password_redbag_time;
            public long? subscription_uin;
            public uint? member_list_change_seq;
            public uint? membercard_seq;
            public long? root_id;
            public long? parent_id;
            public uint? team_seq;
            public long? history_msg_begin_time;
            public long? invite_no_auth_num_limit;
            public uint? cmduin_history_msg_seq;
            public uint? cmduin_join_msg_seq;
            public uint? group_flagext3;
            public uint? group_open_appid;
            public uint? is_conf_group;
            public uint? is_modify_conf_group_face;
            public uint? is_modify_conf_group_name;
            public uint? no_finger_open_flag;
            public uint? no_code_finger_open_flag;
            public uint? auto_agree_join_group_user_num_for_normal_group;
            public uint? auto_agree_join_group_user_num_for_conf_group;
            public uint? is_allow_conf_group_member_nick;
            public uint? is_allow_conf_group_member_at_all;
            public uint? is_allow_conf_group_member_modify_group_name;
            public string long_group_name;
            public uint? cmduin_join_real_msg_seq;
            public uint? is_group_freeze;
            public uint? msg_limit_frequency;
            public byte[] join_group_auth;
            public uint? hl_guild_appid;
            public uint? hl_guild_sub_type;
            public uint? hl_guild_orgid;
            public uint? is_allow_hl_guild_binary;
            public uint? cmduin_ringtone_id;
            public uint? group_flagext4;
            public uint? group_freeze_reason;
            public uint? is_allow_recall_msg;
        }

        public struct TagRecord
        {
            public long? from_uin;    //  8
            public long? group_code;  // 16
            public byte[] tag_id;     // 26
            public long? set_time;    // 32
            public uint? good_num;    // 40
            public uint? bad_num;     // 48
            public uint? tag_len;     // 56
            public byte[] tag_value;  // 66
        }

        public struct GroupGeoInfo
        {
            public long? owneruin;    //  8
            public uint? settime;     // 16
            public uint? cityid;      // 24 
            public long? longitude;   // 32
            public long? latitude;    // 40
            public byte[] geocontent; // 50
            public long? poi_id;      // 56
        }

        public struct GroupHeadPortrait
        {
            public uint? pic_cnt;                      //  8
            public GroupHeadPortraitInfo rpt_msg_info; // 18
            public uint? default_id;                   // 24
            public uint? verifying_pic_cnt;            // 32
            public GroupHeadPortraitInfo rpt_msg_verifyingpic_info; // 42
        }

        public struct GroupHeadPortraitInfo
        {
            public uint? rpt_pic_id; // 8
            public uint? left_x;     // 16
            public uint? left_y;     // 24 
            public uint? right_x;    // 32
            public uint? right_y;    // 40
        }
    }
}
