// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class GroupInviteEvent : FilterableEvent
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
    /// [In] [Out] <br/>
    /// Inviter uin
    /// </summary>
    public uint InviterUin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Inviter nick name
    /// </summary>
    public string InviterNick { get; }

    /// <summary>
    /// [Out] <br/>
    /// Inviter is admin 
    /// </summary>
    public bool InviterIsAdmin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Invite time
    /// </summary>
    public uint InviteTime { get; }

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

    private GroupInviteEvent(uint groupUin, string groupName,
        uint inviterUin, string inviterNick, bool inviterIsAdmin,
        uint inviteTime, long token) : base(0)
    {
        GroupUin = groupUin;
        GroupName = groupName;
        InviterUin = inviterUin;
        InviterNick = inviterNick;
        InviterIsAdmin = inviterIsAdmin;
        InviteTime = inviteTime;
        Token = token;
    }

    private GroupInviteEvent(uint groupUin, uint inviterUin, long token,
        bool isApproved, string declineReason, bool prevent) : base(true)
    {
        GroupUin = groupUin;
        Token = token;
        IsApproved = isApproved;
        DeclineReason = declineReason;
        PreventRequest = prevent;
        InviterUin = inviterUin;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="groupName"></param>
    /// <param name="inviterUin"></param>
    /// <param name="inviterNick"></param>
    /// <param name="inviterIsAdmin"></param>
    /// <param name="inviteTime"></param>
    /// <param name="reqToken"></param>
    /// <returns></returns>
    internal static GroupInviteEvent Push(uint groupUin, string groupName,
        uint inviterUin, string inviterNick, bool inviterIsAdmin, uint inviteTime, long reqToken)
        => new(groupUin, groupName, inviterUin, inviterNick, inviterIsAdmin, inviteTime, reqToken);

    /// <summary>
    /// Construct approve event
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="inviterUin"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal static GroupInviteEvent Approve(uint groupUin, uint inviterUin, long token)
        => new(groupUin, inviterUin, token, true, null, false);

    /// <summary>
    /// Construct decline event
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="inviterUin"></param>
    /// <param name="token"></param>
    /// <param name="reason"></param>
    /// <param name="prevent"></param>
    /// <returns></returns>
    internal static GroupInviteEvent Decline(uint groupUin, uint inviterUin,
        long token, string reason, bool prevent) => new(groupUin, inviterUin, token, false, reason, prevent);
}
