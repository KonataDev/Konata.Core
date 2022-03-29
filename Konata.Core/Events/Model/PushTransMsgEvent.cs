// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class PushTransMsgEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Request Id <br/>
    /// </summary>
    public int RequestId { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Svr Ip <br/>
    /// </summary>
    public int SvrIp { get; }

    private PushTransMsgEvent(int requestId, int svrip) : base(0)
    {
        RequestId = requestId;
        SvrIp = svrip;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="svrip"></param>
    /// <returns></returns>
    internal static PushTransMsgEvent Push(int requestId, int svrip)
        => new(requestId, svrip);
}
