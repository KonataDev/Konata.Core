// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class BotOnlineEvent : BaseEvent
{
    public OnlineType Type { get; }

    private BotOnlineEvent(OnlineType type)
        => Type = type;

    /// <summary>
    /// Online type
    /// </summary>
    public enum OnlineType
    {
        FirstOnline,
        ConnectionReset
    }

    /// <summary>
    /// Construct event create
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static BotOnlineEvent Push(OnlineType type)
        => new(type);
}
