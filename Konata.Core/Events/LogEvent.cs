namespace Konata.Core.Events;

public enum LogLevel
{
    Verbose,
    Information,
    Warning,
    Exception,
    Fatal
}

public class LogEvent : BaseEvent
{
    public string Tag { get; }

    public LogLevel Level { get; }

    private LogEvent(string tag,
        LogLevel level, string content)
    {
        Tag = tag;
        Level = level;
        EventMessage = content;
    }

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="level"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    internal static LogEvent Create(string tag,
        LogLevel level, string content) => new(tag, level, content);
}
