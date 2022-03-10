namespace Konata.Core.Events.Model;

internal class FunCallSvrCallEvent : ProtocolEvent
{
    /// <summary>
    /// Call uin
    /// </summary>
    public uint CallUin { get; }

    private FunCallSvrCallEvent(uint callUin) : base(6000, true)
    {
        CallUin = callUin;
    }
    
    private FunCallSvrCallEvent(int resultCode) 
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="callUin"></param>
    /// <returns></returns>
    public static FunCallSvrCallEvent Create(uint callUin)
        => new(callUin);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    public static FunCallSvrCallEvent Result(int resultCode)
        => new(resultCode);
}
