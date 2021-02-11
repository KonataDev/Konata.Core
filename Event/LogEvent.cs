using System;
using System.Text;

namespace Konata.Core.Event
{
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
        public LogLevel Level { get; set; }

        public string Tag { get; set; }
    }
}
