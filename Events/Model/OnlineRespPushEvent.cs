namespace Konata.Core.Events.Model;

public class OnlineRespPushEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Request Id <br/>
    /// </summary>
    public int RequestId { get; }
    
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Unknown variable 0 <br/>
    /// </summary>
    public int UnknownV0 { get; }
    
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Unknown variable 1 <br/>
    /// </summary>
    public int UnknownV1 { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Svr Ip <br/>
    /// </summary>
    public int SvrIp { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Unknown variable 8 <br/>
    /// </summary>
    public byte[] UnknownV8 { get; }
    
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Unknown variable 32 <br/>
    /// </summary>
    public int UnknownV32 { get; }

    private OnlineRespPushEvent(uint selfUin, OnlineReqPushEvent original) : base(0, false)
    {
        SelfUin = selfUin;
        RequestId = original.RequestId;
        SvrIp = original.SvrIp;
        UnknownV0 = original.UnknownV0;
        UnknownV1 = original.UnknownV1;
        UnknownV8 = original.UnknownV8;
        UnknownV32 = original.UnknownV32;
    }

    /// <summary>
    /// Confirm the push event
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    internal static OnlineRespPushEvent Create(uint selfUin, OnlineReqPushEvent original)
        => new(selfUin, original);
}
