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

    private FriendRequestEvent(uint reqUin, string reqNick, uint reqTime,
        string reqComment, long token) : base(0)
    {
        ReqUin = reqUin;
        ReqNick = reqNick;
        ReqTime = reqTime;
        ReqComment = reqComment;
        Token = token;
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
}
