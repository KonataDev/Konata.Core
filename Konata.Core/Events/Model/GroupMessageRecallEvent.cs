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
    /// <b>[Opt] [Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Source Info <br/>
    /// </summary>
    public SourceInfo SourceInfo { get; }

    private GroupMessageRecallEvent(uint groupUin, SourceInfo sourceInfo) 
        : base(2000, true)
    {
        GroupUin = groupUin;
        SourceInfo = sourceInfo;
    }

    private GroupMessageRecallEvent(int resultCode)
        : base(resultCode)
    {
    }

    private GroupMessageRecallEvent(uint groupUin, uint operatorUin,
        SourceInfo sourceInfo) : base(0)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;
        SourceInfo = sourceInfo;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="sourceInfo"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Create(uint groupUin,
        SourceInfo sourceInfo) => new(groupUin, sourceInfo);

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
    /// <param name="sourceInfo"></param>
    /// <returns></returns>
    internal static GroupMessageRecallEvent Push(uint groupUin, uint operatorUin,
        SourceInfo sourceInfo) => new(groupUin, operatorUin, sourceInfo);
}
