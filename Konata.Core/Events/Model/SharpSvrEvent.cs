using Konata.Core.Utils.IO;

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
    /// Room id
    /// </summary>
    public long RoomId { get; }

    /// <summary>
    /// Ack payload
    /// </summary>
    public ByteBuffer AckPayload { get; }

    /// <summary>
    /// Call status
    /// </summary>
    public CallStatus Status { get; }

    /// <summary>
    /// Call out
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="callUin"></param>
    private SharpSvrEvent(uint selfUin, uint callUin)
        : base(6000, true)
    {
        SelfUin = selfUin;
        CallUin = callUin;
        Status = CallStatus.CallOut;
    }

    /// <summary>
    /// ack
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="e"></param>
    private SharpSvrEvent(uint selfUin, SharpSvrEvent e)
        : base(6000, false)
    {
        SelfUin = selfUin;
        CallUin = e.CallUin;
        Status = CallStatus.Ack;
        RoomId = e.RoomId;
    }

    /// <summary>
    /// ack msf
    /// </summary>
    /// <param name="payload"></param>
    private SharpSvrEvent(ByteBuffer payload)
        : base(6000, false)
    {
        Status = CallStatus.AckMsf;
        AckPayload = payload;
    }

    /// <summary>
    /// Push ack
    /// </summary>
    /// <param name="status"></param>
    /// <param name="roomId"></param>
    /// <param name="friendUin"></param>
    private SharpSvrEvent(CallStatus status, long roomId, uint friendUin)
        : base(0)
    {
        Status = status;
        RoomId = roomId;
        CallUin = friendUin;
    }

    /// <summary>
    /// Result
    /// </summary>
    /// <param name="resultCode"></param>
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
    /// <param name="selfUin"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SharpSvrEvent Ack(uint selfUin, SharpSvrEvent e)
        => new(selfUin, e);

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    public static SharpSvrEvent AckMsf(ByteBuffer payload)
        => new(payload);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    public static SharpSvrEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct ack
    /// </summary>
    /// <param name="status"></param>
    /// <param name="roomId"></param>
    /// <param name="friendUin"></param>
    /// <returns></returns>
    public static SharpSvrEvent PushAck(CallStatus status, long roomId, uint friendUin)
        => new(status, roomId, friendUin);

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    public static SharpSvrEvent PushAckMsf(ByteBuffer payload)
        => new(payload);

    public enum CallStatus
    {
        Ack = 2,
        CallAccepted = 10,
        Unknown5 = 5,
        CallIn = 100,
        CallOut = 101,
        AckMsf = 102,
    }
}
