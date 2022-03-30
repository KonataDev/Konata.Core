using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class GroupMessageRecallEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint AffectedUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint Sequence { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint Random { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint Time { get; }

    /// <summary>
    /// Recalling a message
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="messageStruct"></param>
    private GroupMessageRecallEvent(uint groupUin, MessageStruct messageStruct)
        : base(true)
    {
        GroupUin = groupUin;
        Sequence = messageStruct.Sequence;
        Random = messageStruct.Random;
        Time = messageStruct.Time;
    }
    
    /// <summary>
    /// Recalling a message
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="sequence"></param>
    /// /// <param name="random"></param>
    private GroupMessageRecallEvent(uint groupUin, uint sequence, uint random)
        : base(true)
    {
        GroupUin = groupUin;
        Sequence = sequence;
        Random = random;
    }

    /// <summary>
    /// Recall result
    /// </summary>
    /// <param name="resultCode"></param>
    private GroupMessageRecallEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Push recall
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="affectedUin"></param>
    /// <param name="sequence"></param>
    /// <param name="rand"></param>
    /// <param name="time"></param>
    private GroupMessageRecallEvent(uint groupUin, uint operatorUin, uint affectedUin,
        uint sequence, uint rand, uint time) : base(0)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;
        AffectedUin = affectedUin;
        Sequence = sequence;
        Random = rand;
        Time = time;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="messageStruct"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Create(uint groupUin,
        MessageStruct messageStruct) => new(groupUin, messageStruct);
    
    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="sequence"></param>
    /// /// <param name="random"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Create(uint groupUin,
        uint sequence, uint random) => new(groupUin, sequence, random);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="affectedUin"></param>
    /// <param name="sequence"></param>
    /// <param name="rand"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Push(uint groupUin, uint operatorUin, uint affectedUin, uint sequence, uint rand, uint time)
        => new(groupUin, operatorUin, affectedUin, sequence, rand, time);
}
