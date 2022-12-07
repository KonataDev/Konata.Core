// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class GroupPokeEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>s
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Member uin <br/>
    /// </summary>
    public uint MemberUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Action prefix string <br/>
    /// </summary>
    public string ActionPrefix { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Action suffix string <br/>
    /// </summary>
    public string ActionSuffix { get; }

    private GroupPokeEvent(uint groupUin, uint memberUin) : base(true)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
    }

    private GroupPokeEvent(uint groupUin, uint memberUin, uint operatorUin,
        string actionPrefix, string actionSuffix) : base(0)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        OperatorUin = operatorUin;
        ActionPrefix = actionPrefix;
        ActionSuffix = actionSuffix;
    }

    private GroupPokeEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="actionPrefix"></param>
    /// <param name="actionSuffix"></param>
    /// <returns></returns>
    internal static GroupPokeEvent Push(uint groupUin, uint memberUin, uint operatorUin,
        string actionPrefix, string actionSuffix) => new(groupUin, memberUin, operatorUin, actionPrefix, actionSuffix);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <returns></returns>
    internal static GroupPokeEvent Create(uint groupUin, uint memberUin)
        => new(groupUin, memberUin);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupPokeEvent Result(int resultCode)
        => new(resultCode);
}
