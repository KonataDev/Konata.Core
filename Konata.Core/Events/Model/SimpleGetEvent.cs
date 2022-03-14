// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

internal class SimpleGetEvent : ProtocolEvent
{
    /// <summary>
    /// Time
    /// </summary>
    public int UnknownTime { get; }

    /// <summary>
    /// Time
    /// </summary>
    public int UnknownTimeB { get; }

    /// <summary>
    /// Ip
    /// </summary>
    public string Ip { get; }

    private SimpleGetEvent() : base(6000, true)
    {
    }

    private SimpleGetEvent(int resultCode, int unknownTime,
        int timeb, string ip) : base(resultCode)
    {
        Ip = ip;
        UnknownTime = unknownTime;
        UnknownTimeB = timeb;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <returns></returns>
    internal static SimpleGetEvent Create() => new();

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="time"></param>
    /// <param name="timeb"></param>
    /// <param name="ip"></param>
    /// <returns></returns>
    internal static SimpleGetEvent Result(int resultCode, int time,
        int timeb, string ip) => new(resultCode, time, timeb, ip);
}
