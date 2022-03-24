namespace Konata.Core.Events.Model;

internal class CorrectTimeEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Server time <br/>
    /// </summary>
    public uint ServerTime { get; }

    private CorrectTimeEvent() : base(true)
    {
    }

    private CorrectTimeEvent(uint time) : base(0)
    {
        ServerTime = time;
    }


    /// <summary>
    /// Construct event request
    /// </summary>
    /// <returns></returns>
    internal static CorrectTimeEvent Create()
        => new();

    internal static CorrectTimeEvent Result(uint time)
        => new(time);
}
