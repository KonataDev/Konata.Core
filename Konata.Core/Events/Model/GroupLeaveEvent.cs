// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class GroupLeaveEvent : ProtocolEvent
{
    /// <summary>
    /// Dismiss the group
    /// </summary>
    public bool Dismiss { get; }

    /// <summary>
    /// Group uin
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// Self uin
    /// </summary>
    public uint SelfUin { get; }

    private GroupLeaveEvent(uint groupUin,
        uint selfUin, bool dismiss) : base(true)
    {
        GroupUin = groupUin;
        SelfUin = selfUin;
        Dismiss = dismiss;
    }

    private GroupLeaveEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct request event
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="dismiss"></param>
    /// <returns></returns>
    public static GroupLeaveEvent Create(uint groupUin,
        uint selfUin, bool dismiss) => new(groupUin, selfUin, dismiss);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupLeaveEvent Result(int resultCode)
        => new(resultCode);
}
