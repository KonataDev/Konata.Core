namespace Konata.Core.Events.Model;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class FriendMessageRecallEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Friend uin <br/>
    /// </summary>
    public uint FriendUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Sequence <br/>
    /// </summary>
    public uint Sequence { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Random <br/>
    /// </summary>
    public uint Random { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Uuid <br/>
    /// </summary>
    public long Uuid { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Time <br/>
    /// </summary>
    public uint Time { get; }

    private FriendMessageRecallEvent(uint friendUin,
        uint operatorUin, uint sequence, uint random, long uuid, uint time) : base(true)
    {
        FriendUin = friendUin;
        OperatorUin = operatorUin;
        Sequence = sequence;
        Random = random;
        Uuid = uuid;
        Time = time;
    }

    private FriendMessageRecallEvent(uint friendUin,
        uint sequence, uint random, long uuid, uint time) : base(true)
    {
        FriendUin = friendUin;
        Sequence = sequence;
        Random = random;
        Uuid = uuid;
        Time = time;
    }

    private FriendMessageRecallEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="sequence"></param>
    /// <param name="random"></param>
    /// <param name="uuid"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Create(uint friendUin,
        uint sequence, uint random, long uuid, uint time) => new(friendUin, sequence, random, uuid, time);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="sequence"></param>
    /// <param name="rand"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Push(uint friendUin, uint operatorUin, uint sequence, uint rand, long uuid, uint time)
        => new(friendUin, operatorUin, sequence, rand, uuid, time);
}
