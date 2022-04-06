// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class ForceOfflineEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Highway host server
    /// </summary>
    public string NotifyTitle { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Offline reason string
    /// </summary>
    public string OfflineReason { get; }

    internal ForceOfflineEvent(string notifyTitle,
        string offlineReason) : base(0)
    {
        NotifyTitle = notifyTitle;
        OfflineReason = offlineReason;
    }
}
