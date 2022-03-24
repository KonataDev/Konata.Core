namespace Konata.Core.Events.Model;

public class GroupKickMemberEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group uin being operated.
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Member uin being operated.
    /// </summary>
    public uint MemberUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Member uin did the operation.
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[Opt] [In]</b> <br/>
    /// Flag to prevent member request or no. <br/>
    /// The default value is <b>false</b>
    /// </summary>
    public bool ToggleType { get; }

    private GroupKickMemberEvent(uint groupUin,
        uint memberUin, uint operatorUin, bool toggleType) : base(true)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        OperatorUin = operatorUin;
        ToggleType = toggleType;
    }

    private GroupKickMemberEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>
    internal static GroupKickMemberEvent Push(uint groupUin, uint memberUin, uint operatorUin, bool toggleType)
        => new(groupUin, memberUin, operatorUin, toggleType);

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>
    internal static GroupKickMemberEvent Create(uint groupUin, uint memberUin, uint operatorUin, bool toggleType)
        => new(groupUin, memberUin, operatorUin, toggleType);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupKickMemberEvent Result(int resultCode)
        => new(resultCode);
}
