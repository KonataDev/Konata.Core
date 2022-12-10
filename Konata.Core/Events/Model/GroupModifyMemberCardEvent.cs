namespace Konata.Core.Events.Model;

public class GroupModifyMemberCardEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> s<br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Member uin <br/>
    /// </summary>
    public uint MemberUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Member group card <br/>
    /// </summary>
    public string MemberCard { get; }

    private GroupModifyMemberCardEvent(uint groupUin,
        uint memberUin, string memberCard) : base(true)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        MemberCard = memberCard;
    }

    private GroupModifyMemberCardEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="memberCard"></param>
    /// <returns></returns>
    internal static GroupModifyMemberCardEvent Create(uint groupUin,
        uint memberUin, string memberCard) => new(groupUin, memberUin, memberCard);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupModifyMemberCardEvent Result(int resultCode)
        => new(resultCode);
}
