namespace Konata.Core.Events.Model;

public class OnlineStatusEvent : ProtocolEvent
{
    public enum Type
    {
        Online = 11,
        Offline = 21,
        Leave = 31,
        Hidden = 41,
        Busy = 50,
        QMe = 60,
        DoNotDisturb = 70,
    }

    public enum SubType
    {
        Normal = 0,
        Offline = 255,

        BatteryPercent = 1000,
        LowSignal = 1011,
        StudyingOnline = 1024,
        TravelAtHome = 1025,
        TiMing = 1027,
        Sleeping = 1016,
        Gaming = 1017,
        Studying = 1018,
        Eating = 1019,
        Soap = 1021,
        Holiday = 1022,
        StayingUpLate = 1032,
        PlayingBall = 1050,
        FallinLove = 1051,
        IMIdle = 1052,
        ListeningMusic = 1028
    }

    /// <summary>
    /// <b>[In]</b>          <br/>
    ///   Online main type   <br/>
    /// </summary>
    public Type EventType { get; }

    /// <summary>
    /// <b>[Opt] [In]</b>   <br/>
    /// Online sub type   <br/>
    ///  - Only valid in <b>OnlineType.Online</b>
    /// </summary>
    public SubType EventSubType { get; }

    /// <summary>
    /// <b>[Opt] [In]</b>   <br/>
    /// Battery percent.  <br/>
    ///   - Only valid in OnlineType.Online with SubType.BatteryPercent
    /// </summary>
    public byte BatteryPercent { get; }

    /// <summary>
    /// <b>[Opt] [In]</b> <br/>
    /// Kick PC while login <br/>
    /// </summary>
    public bool IsKickPC { get; }

    private OnlineStatusEvent(Type eventType, SubType subType)
        : base(true)
    {
    }

    private OnlineStatusEvent(Type eventType) : base(0)
    {
        EventType = eventType;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="subType"></param>
    /// <returns></returns>
    internal static OnlineStatusEvent Create(Type eventType,
        SubType subType) => new(eventType, subType);

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    internal static OnlineStatusEvent Create(Type eventType)
        => new(eventType) {WaitForResponse = true};

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    internal static OnlineStatusEvent Result(Type eventType)
        => new(eventType);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="reasonStr"></param>
    /// <returns></returns>
    internal static OnlineStatusEvent Push(Type eventType, string reasonStr)
        => new(eventType) {EventMessage = reasonStr};
}
