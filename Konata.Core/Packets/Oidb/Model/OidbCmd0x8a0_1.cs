﻿using System;

namespace Konata.Core.Packets.Oidb.Model;

/// <summary>
/// 移除單個群成員
/// </summary>
internal class OidbCmd0x8a0_1 : OidbCmd0x8a0
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
