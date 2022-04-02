// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class ReqSystemMsgGroupEvent : ProtocolEvent
{
    /// <summary>
    /// [In]  <br/>
    /// Number of requested
    /// </summary>
    public int RequestNum { get; }

    /// <summary>
    /// [In]  <br/>
    /// Requested message type
    /// </summary>
    public ReqMsgType RequestMsgType { get; }

    public enum ReqMsgType
    {
        Normal = 1,
        Risk = 2
    }

    private ReqSystemMsgGroupEvent(int resultCode)
        : base(resultCode)
    {
    }

    private ReqSystemMsgGroupEvent(int num, ReqMsgType reqType)
        : base(true)
    {
        RequestNum = num;
        RequestMsgType = reqType;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <returns></returns>
    public static ReqSystemMsgGroupEvent Create(int num = 20,
        ReqMsgType reqType = ReqMsgType.Normal) => new(num, reqType);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    public static ReqSystemMsgGroupEvent Result(int resultCode)
        => new(resultCode);
}
