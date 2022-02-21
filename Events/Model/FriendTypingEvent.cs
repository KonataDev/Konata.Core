namespace Konata.Core.Events.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
public class FriendTypingEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Group uin <br/>
    /// </summary>s
    public uint FriendUin { get; }

    private FriendTypingEvent(uint friendUin) : base(0)
    {
        FriendUin = friendUin;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="friendUin"></param>
    /// <returns></returns>
    internal static FriendTypingEvent Push(uint friendUin) => new(friendUin);
}
