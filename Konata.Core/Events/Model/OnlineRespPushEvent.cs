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
    /// From source <br/>
    /// </summary>
    public uint FromSource { get; }
    
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
        FromSource = original.FromSource;
        UnknownV1 = original.UnknownV1;
        UnknownV8 = original.UnknownV8;
        UnknownV32 = original.UnknownV32;
    }

    private OnlineRespPushEvent(uint selfUin, PushTransMsgEvent original) : base(0, false)
    {
        SelfUin = selfUin;
        RequestId = original.RequestId;
        SvrIp = original.SvrIp;
    }

    /// <summary>
    /// Confirm the push event
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    internal static OnlineRespPushEvent Create(uint selfUin, OnlineReqPushEvent original)
        => new(selfUin, original);

    /// <summary>
    /// Confirm the push event
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    internal static OnlineRespPushEvent Create(uint selfUin, PushTransMsgEvent original)
        => new(selfUin, original);
}
