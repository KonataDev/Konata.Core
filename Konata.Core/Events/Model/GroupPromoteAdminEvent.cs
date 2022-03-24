namespace Konata.Core.Events.Model;

public class GroupPromoteAdminEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Group uin being operated <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Member uin being operated <br/>
    /// </summary>
    public uint MemberUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Flag to toggle set or unset <br/>
    /// </summary>
    public bool ToggleType { get; }

    private GroupPromoteAdminEvent(uint groupUin,
        uint memberUin, bool toggleType) : base(true)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        ToggleType = toggleType;
    }

    private GroupPromoteAdminEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>
    internal static GroupPromoteAdminEvent Push(uint groupUin,
        uint memberUin, bool toggleType) => new(groupUin, memberUin, toggleType);

    /// <summary>
    /// Construct event requests
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>
    internal static GroupPromoteAdminEvent Create(uint groupUin,
        uint memberUin, bool toggleType) => new(groupUin, memberUin, toggleType);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupPromoteAdminEvent Result(int resultCode)
        => new(resultCode);
}
