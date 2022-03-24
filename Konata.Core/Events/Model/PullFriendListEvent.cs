using System.Collections.Generic;
using Konata.Core.Common;

namespace Konata.Core.Events.Model;

public class PullFriendListEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Start index <br/>
    /// </summary>
    public uint StartIndex { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Limit length <br/>
    /// </summary>
    public uint LimitNum { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Error code <br/>
    /// </summary>
    public short ErrorCode { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Total friend count <br/>
    /// </summary>
    public uint TotalFriendCount { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Friend info list s<br/>
    /// </summary>
    public List<BotFriend> FriendInfo { get; }

    private PullFriendListEvent(uint selfUin,
        uint startIndex, uint limitNum) : base(true)
    {
        SelfUin = selfUin;
        StartIndex = startIndex;
        LimitNum = limitNum;
    }

    private PullFriendListEvent(int resultCode, short errorCode,
        List<BotFriend> friendInfo, uint totalFriendCount) : base(resultCode)
    {
        ErrorCode = errorCode;
        FriendInfo = friendInfo;
        TotalFriendCount = totalFriendCount;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="startIndex"></param>
    /// <param name="limitNum"></param>
    /// <returns></returns>
    internal static PullFriendListEvent Create(uint selfUin,
        uint startIndex, uint limitNum) => new(selfUin, startIndex, limitNum);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="errorCode"></param>
    /// <param name="friendInfo"></param>
    /// <param name="totalFriendCount"></param>
    /// <returns></returns>
    internal static PullFriendListEvent Result(int resultCode, short errorCode, List<BotFriend> friendInfo,
        uint totalFriendCount) => new(resultCode, errorCode, friendInfo, totalFriendCount);
}
