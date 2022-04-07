// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class GroupMemberIncreasedEvent : ProtocolEvent
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
    /// Member nick did the operation.
    /// </summary>
    public string MemberNick { get; }

    private GroupMemberIncreasedEvent(uint groupUin,
        uint memberUin, string memberNick) : base(0)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        MemberNick = memberNick;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="memberNick"></param>
    /// <returns></returns>
    internal static GroupMemberIncreasedEvent Push(uint groupUin, uint memberUin, string memberNick)
        => new(groupUin, memberUin, memberNick);
}
