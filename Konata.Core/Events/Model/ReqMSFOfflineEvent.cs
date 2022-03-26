// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

internal class ReqMSFOfflineEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Highway host server
    /// </summary>
    internal string NotifyTitle { get; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Offline reason string
    /// </summary>
    internal string OfflineReason { get; }

    internal ReqMSFOfflineEvent(string notifyTitle,
        string offlineReason) : base(0)
    {
        NotifyTitle = notifyTitle;
        OfflineReason = offlineReason;
    }
}
