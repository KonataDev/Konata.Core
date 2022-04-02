// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class ReqSystemMsgFriendEvent : ProtocolEvent
{
    /// <summary>
    /// [In]  <br/>
    /// Number of requested
    /// </summary>
    public int RequestNum { get; }

    private ReqSystemMsgFriendEvent(int resultCode)
        : base(resultCode)
    {
    }

    private ReqSystemMsgFriendEvent(int num, object x)
        : base(true)
    {
        RequestNum = num;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <returns></returns>
    public static ReqSystemMsgFriendEvent Create(int num = 20)
        => new(num, null);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    public static ReqSystemMsgFriendEvent Result(int resultCode)
        => new(resultCode);
}
