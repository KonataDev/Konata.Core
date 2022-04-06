// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class BotOfflineEvent : BaseEvent
{
    /// <summary>
    /// [Out] <br/>
    /// Offline type
    /// </summary>
    public OfflineType Type { get; }

    private BotOfflineEvent(OfflineType type, string message)
    {
        Type = type;
        EventMessage = message;
    }

    /// <summary>
    /// Offline type
    /// </summary>
    public enum OfflineType
    {
        UserLoggedOut,
        ServerKickOff,
        NetworkDown
    }

    /// <summary>
    /// Construct event create
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    internal static BotOfflineEvent Push(OfflineType type, string message)
        => new(type, message);
}
