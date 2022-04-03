// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class FriendRequestEvent : ProtocolEvent
{
    /// <summary>
    /// [Out] <br/>
    /// Request uin
    /// </summary>
    public uint ReqUin { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request nickname
    /// </summary>
    public string ReqNick { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request time
    /// </summary>
    public uint ReqTime { get; }

    /// <summary>
    /// [Out] <br/>
    /// Request comment
    /// </summary>
    public string ReqComment { get; }

    /// <summary>
    /// [Out] <br/>
    /// Token for accept or reject
    /// </summary>
    public long Token { get; }
    
    /// <summary>
    /// [In] [Opt] <br/>
    /// Is approved
    /// </summary>
    internal bool IsApproved { get; }

    /// <summary>
    /// [In] [Opt] <br/>
    /// Prevent request from this account
    /// </summary>
    internal bool PreventRequest { get; }

    private FriendRequestEvent(uint reqUin, string reqNick, uint reqTime,
        string reqComment, long token) : base(0)
    {
        ReqUin = reqUin;
        ReqNick = reqNick;
        ReqTime = reqTime;
        ReqComment = reqComment;
        Token = token;
    }
    
    private FriendRequestEvent(uint reqUin, long token,
        bool isApproved, bool prevent) : base(true)
    {
        ReqUin = reqUin;
        Token = token;
        IsApproved = isApproved;
        PreventRequest = prevent;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="reqUin"></param>
    /// <param name="reqNick"></param>
    /// <param name="reqTime"></param>
    /// <param name="reqComment"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static FriendRequestEvent Push(uint reqUin, string reqNick, uint reqTime,
        string reqComment, long token) => new(reqUin, reqNick, reqTime, reqComment, token);
    
    /// <summary>
    /// Construct approve event
    /// </summary>
    /// <param name="reqUin"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    internal static FriendRequestEvent Approve(uint reqUin, long token)
        => new(reqUin, token, true, false);

    /// <summary>
    /// Construct decline event
    /// </summary>
    /// <param name="reqUin"></param>
    /// <param name="token"></param>
    /// <param name="prevent"></param>
    /// <returns></returns>
    internal static FriendRequestEvent Decline(uint reqUin, long token,
        bool prevent) => new(reqUin, token, false, prevent);
}
