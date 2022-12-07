// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class FriendPokeEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Friend uin <br/>
    /// </summary>s
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Friend uin <br/>
    /// </summary>s
    public uint FriendUin { get; }

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

    private FriendPokeEvent(int resultCode) : base(resultCode)
    {
    }

    private FriendPokeEvent(uint selfUin, uint friendUin) : base(true)
    {
        SelfUin = selfUin;
        FriendUin = friendUin;
    }

    private FriendPokeEvent(uint friendUin,
        string actionPrefix, string actionSuffix) : base(0)
    {
        FriendUin = friendUin;
        ActionPrefix = actionPrefix;
        ActionSuffix = actionSuffix;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="actionPrefix"></param>
    /// <param name="actionSuffix"></param>
    /// <returns></returns>
    internal static FriendPokeEvent Push(uint friendUin, string actionPrefix,
        string actionSuffix) => new(friendUin, actionPrefix, actionSuffix);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="friendUin"></param>
    /// <returns></returns>
    internal static FriendPokeEvent Create(uint selfUin, uint friendUin)
        => new(selfUin, friendUin);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static FriendPokeEvent Result(int resultCode)
        => new(resultCode);
}
