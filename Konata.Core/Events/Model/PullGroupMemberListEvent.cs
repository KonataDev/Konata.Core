using System.Collections.Generic;
using Konata.Core.Common;

namespace Konata.Core.Events.Model;

public class PullGroupMemberListEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group code <br/>
    /// </summary>
    public ulong GroupCode { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Next uin <br/>
    /// </summary>
    public uint NextUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Office mode <br/>
    /// </summary>
    public uint OfficeMode { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Time for next get op <br/>
    /// </summary>
    public uint NextGetTime { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Error code <br/>
    /// </summary>
    public short ErrorCode { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Partial member list <br/>
    /// </summary>
    public List<BotMember> MemberInfo { get; }

    private PullGroupMemberListEvent(uint selfUin, uint groupUin,
        ulong groupCode, uint nextUin) : base(6000, true)
    {
        SelfUin = selfUin;
        GroupUin = groupUin;
        GroupCode = groupCode;
        NextUin = nextUin;
    }

    private PullGroupMemberListEvent(int resultCode, short errorCode, uint groupUin,
        ulong groupCode, List<BotMember> memberInfo, uint nextUin) : base(resultCode)
    {
        ErrorCode = errorCode;
        GroupUin = groupUin;
        GroupCode = groupCode;
        MemberInfo = memberInfo;
        NextUin = nextUin;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="groupUin"></param>
    /// <param name="groupCode"></param>
    /// <param name="nextUin"></param>
    /// <returns></returns>
    internal static PullGroupMemberListEvent Create(uint selfUin, uint groupUin,
        ulong groupCode, uint nextUin) => new(selfUin, groupUin, groupCode, nextUin);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="errorCode"></param>
    /// <param name="groupUin"></param>
    /// <param name="groupCode"></param>
    /// <param name="memberInfo"></param>
    /// <param name="nextUin"></param>
    /// <returns></returns>
    internal static PullGroupMemberListEvent Result(int resultCode, short errorCode, uint groupUin,
        ulong groupCode, List<BotMember> memberInfo, uint nextUin) => new(resultCode, errorCode,
        groupUin, groupCode, memberInfo, nextUin);
}
