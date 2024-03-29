﻿using System.Collections.Generic;
using Konata.Core.Common;

namespace Konata.Core.Events.Model;

internal class PullGroupListEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Group info list <br/>
    /// </summary>
    public List<BotGroup> GroupInfo { get; }

    private PullGroupListEvent(uint selfUin)
        : base(true)
    {
        SelfUin = selfUin;
    }

    private PullGroupListEvent(int resultCode,
        List<BotGroup> groupInfo) : base(resultCode)
    {
        GroupInfo = groupInfo;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="selfUin"></param>
    /// <returns></returns>
    internal static PullGroupListEvent Create(uint selfUin)
        => new(selfUin);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="groupInfo"></param>
    /// <returns></returns>
    internal static PullGroupListEvent Result(int resultCode,
        List<BotGroup> groupInfo) => new(resultCode, groupInfo);
}
