// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class OnlineReqPushEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Request Id <br/>
    /// </summary>
    public int RequestId { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Inner event
    /// </summary>
    public ProtocolEvent InnerEvent { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// From source <br/>
    /// </summary>
    public uint FromSource { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Unknown variable 1 <br/>
    /// </summary>
    public int UnknownV1 { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Svr Ip <br/>
    /// </summary>
    public int SvrIp { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Unknown variable 8 <br/>
    /// </summary>
    public byte[] UnknownV8 { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Unknown variable 32 <br/>
    /// </summary>
    public int UnknownV32 { get; }

    private OnlineReqPushEvent(ProtocolEvent innerEvent,
        int requestId, uint fromSource, int v1, int svrip, byte[] v8, int v32) : base(0)
    {
        RequestId = requestId;
        InnerEvent = innerEvent;
        SvrIp = svrip;
        UnknownV1 = v1;
        UnknownV8 = v8;
        FromSource = fromSource;
        UnknownV32 = v32;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="innerEvent"></param>
    /// <param name="requestId"></param>
    /// <param name="fromSource"></param>
    /// <param name="v1"></param>
    /// <param name="svrip"></param>
    /// <param name="v8"></param>
    /// <param name="v32"></param>
    /// <returns></returns>
    internal static OnlineReqPushEvent Push(ProtocolEvent innerEvent,
        int requestId, uint fromSource, int v1, int svrip, byte[] v8, int v32)
        => new(innerEvent, requestId, fromSource, v1, svrip, v8, v32);
}
