using Konata.Core.Common;
using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Konata.Core.Events.Model;

public class GroupRequestJoinEvent : FilterableEvent
{
    /// <summary>
    /// [Out] <br/>
    /// Group uin
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Group name
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// [Out] <br/>
    /// Inviter uin
    /// </summary>
    public uint InviterUin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request uin
    /// </summary>
    public uint ReqUin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request nick name
    /// </summary>
    public string ReqNick { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request time
    /// </summary>
    public uint ReqTime { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request message
    /// </summary>
    public string ReqComment { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request token for accept or reject
    /// </summary>
    public long Token { get; }

    /// <summary>
    /// [In] [Opt] <br/>
    /// Is approved
    /// </summary>
    internal bool IsApproved { get; }

    /// <summary>
    /// [In] [Opt] <br/>
    /// Decline reason
    /// </summary>
    internal string DeclineReason { get; }

    /// <summary>
    /// [In] [Opt] <br/>
    /// Prevent request from this account
    /// </summary>
    internal bool PreventRequest { get; }

    private GroupRequestJoinEvent(uint groupUin, string groupName,
        uint inviterUin, uint reqUin, string reqNick, string reqComment,
        uint reqTime, long token) : base(0)
    {
        GroupUin = groupUin;
        GroupName = groupName;
        ReqUin = reqUin;
        ReqNick = reqNick;
        ReqTime = reqTime;
        ReqComment = reqComment;
        InviterUin = inviterUin;
        Token = token;
    }

    private GroupRequestJoinEvent(uint groupUin, uint reqUin, long token,
        bool isApproved, string declineReason, bool prevent) : base(true)
    {
        GroupUin = groupUin;
        Token = token;
        IsApproved = isApproved;
        DeclineReason = declineReason;
        PreventRequest = prevent;
        ReqUin = reqUin;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="groupName"></param>
    /// <param name="inviterUin"></param>
    /// <param name="reqUin"></param>
    /// <param name="reqNick"></param>
    /// <param name="reqComment"></param>
    /// <param name="reqTime"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal static GroupRequestJoinEvent Push(uint groupUin, string groupName, uint inviterUin,
        uint reqUin, string reqNick, string reqComment, uint reqTime, long token)
        => new(groupUin, groupName, inviterUin, reqUin, reqNick, reqComment, reqTime, token);

    /// <summary>
    /// Construct approve event
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="reqUin"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal static GroupRequestJoinEvent Approve(uint groupUin, uint reqUin, long token)
        => new(groupUin, reqUin, token, true, null, false);

    /// <summary>
    /// Construct decline event
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="reqUin"></param>
    /// <param name="token"></param>
    /// <param name="reason"></param>
    /// <param name="prevent"></param>
    /// <returns></returns>
    internal static GroupRequestJoinEvent Decline(uint groupUin, uint reqUin,
        long token, string reason, bool prevent) => new(groupUin, reqUin, token, false, reason, prevent);
}
