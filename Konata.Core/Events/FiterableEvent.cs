// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events;

public class FilterableEvent : ProtocolEvent
{
    internal uint FilterTime { get; private set; }

    internal uint FilterId { get; private set; }

    internal FilterableEvent(int rsultCode) : base(rsultCode)
    {
    }

    internal FilterableEvent(bool wait) : base(wait)
    {
    }

    internal void SetFilterIdenfidentor(uint filterTime, uint any)
    {
        FilterTime = filterTime;
        FilterId = any;
    }

    internal long GetFilterIdenfidentor()
        => (long) FilterTime << 32 | FilterId;
}
