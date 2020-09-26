using System;

namespace Konata.Msf
{
    public enum EventType : uint
    {
        Idle = uint.MaxValue,

        DoLogin = 0x1000,
        DoHeartBeat,

        OnBotStart = 0x2000,
        OnVerifySliderCaptcha,
        OnVerifyImageCaptcha,
        OnVerifySmsCaptcha,
        OnPrivateMessage,
        OnGroupMessage,
    }

    public class Event
    {
        public readonly EventType _type;
        public readonly object[] _args;

        public Event(EventType type, params object[] args)
        {
            _type = type;
            _args = args;
        }
    }
}
