namespace Konata.Core.Events.Model;

internal class GroupMessageReadEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Request id <br/>
    /// </summary>
    public uint RequestId { get; }

    private GroupMessageReadEvent(uint groupUin,
        uint requestId, int sessionSeq) : base(false)
    {
        GroupUin = groupUin;
        RequestId = requestId;
        SessionSequence = sessionSeq;
    }

    private GroupMessageReadEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="requestId"></param>
    /// <param name="sessionSeq"></param>
    /// <returns></returns>
    internal static GroupMessageReadEvent Create(uint groupUin,
        uint requestId, int sessionSeq) => new(groupUin, requestId, sessionSeq);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupMessageReadEvent Result(int resultCode)
        => new(resultCode);
}
