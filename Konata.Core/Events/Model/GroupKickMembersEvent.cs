namespace Konata.Core.Events.Model;

public class GroupKickMembersEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Members uin <br/>
    /// </summary>
    public uint[] MembersUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Kick type <br/>
    /// </summary>
    public bool ToggleType { get; }

    private GroupKickMembersEvent(uint groupUin,
        uint[] membersUin, bool toggleType) : base(2000, true)
    {
        GroupUin = groupUin;
        MembersUin = membersUin;
        ToggleType = toggleType;
    }

    private GroupKickMembersEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="membersUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>s
    internal GroupKickMembersEvent Create(uint groupUin, uint[] membersUin,
        bool toggleType) => new(groupUin, membersUin, toggleType);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupKickMembersEvent Result(int resultCode)
        => new(resultCode);
}
