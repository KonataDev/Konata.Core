// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class GroupMuteMemberEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[Opt] [In] [Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint MemberUin { get; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Mute time seconds <br/>
    /// </summary>
    public uint TimeSeconds { get; }

    private GroupMuteMemberEvent(uint groupUin,
        uint memberUin, uint timeSeconds) : base(true)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        TimeSeconds = timeSeconds;
    }

    private GroupMuteMemberEvent(int resultCode)
        : base(resultCode)
    {
    }

    private GroupMuteMemberEvent(uint groupUin, uint memberUin,
        uint operatorUin, uint timeSeconds) : base(0)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        TimeSeconds = timeSeconds;
        OperatorUin = operatorUin;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="timeSeconds"></param>
    /// <returns></returns>
    internal static GroupMuteMemberEvent Create(uint groupUin, uint memberUin,
        uint timeSeconds) => new(groupUin, memberUin, timeSeconds != 0 ? timeSeconds : uint.MaxValue);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupMuteMemberEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="timeSeconds"></param>
    /// <returns></returns>
    internal static GroupMuteMemberEvent Push(uint groupUin, uint memberUin, uint operatorUin,
        uint timeSeconds) => new(groupUin, memberUin, operatorUin, timeSeconds);
}
