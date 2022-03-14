// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class GroupSettingsAnonymousEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Operator uin <br/>
    /// </summary>
    public uint OperatorUin { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Toggle type <br/>
    /// </summary>
    public bool ToggleType { get; }

    private GroupSettingsAnonymousEvent(uint groupUin,
        uint operatorUin, bool toggleType) : base(0)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;
        ToggleType = toggleType;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="operatorUin"></param>
    /// <param name="toggleType"></param>
    /// <returns></returns>
    internal static GroupSettingsAnonymousEvent Push(uint groupUin, uint operatorUin,
        bool toggleType) => new(groupUin, operatorUin, toggleType);
}
