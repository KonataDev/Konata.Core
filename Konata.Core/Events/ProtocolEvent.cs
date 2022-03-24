// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Konata.Core.Events;

public class ProtocolEvent : BaseEvent
{
    public int ResultCode { get; protected set; }

    public int SessionSequence { get; protected set; }

    public bool WaitForResponse { get; protected set; }

    internal ProtocolEvent(bool wait)
        => WaitForResponse = wait;

    internal ProtocolEvent(int resultCode)
        => ResultCode = resultCode;

    /// <summary>
    /// Set session sequence
    /// </summary>
    /// <param name="sessionSeq"></param>
    internal void SetSessionSequence(int sessionSeq)
        => SessionSequence = sessionSeq;
}
