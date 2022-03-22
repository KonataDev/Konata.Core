// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class SharpSvrEvent : ProtocolEvent
{
    /// <summary>
    /// Self uin
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// Call uin
    /// </summary>
    public uint CallUin { get; }

    /// <summary>
    /// Call status
    /// </summary>
    public CallStatus Status { get; }

    private SharpSvrEvent(uint selfUin, uint callUin)
        : base(6000, true)
    {
        SelfUin = selfUin;
        CallUin = callUin;
        Status = CallStatus.CallOut;
    }

    private SharpSvrEvent(int resultCode)
        : base(resultCode)
    {
    }
    
    private SharpSvrEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="callUin"></param>
    /// <returns></returns>
    public static SharpSvrEvent CallOut(uint selfUin, uint callUin)
        => new(selfUin, callUin);

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SharpSvrEvent Ack(SharpSvrEvent e)
        => new();
    
    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    public static SharpSvrEvent Result(int resultCode)
        => new(resultCode);

    public static SharpSvrEvent PushAck(long roomId, uint friendUin)
        => 
    
    
    public enum CallStatus
    {
        Ack,
        CallIn,
        CallOut,
    }
}
