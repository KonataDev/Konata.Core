using System;

namespace Konata.Msf
{
    public enum EventType : uint
    {
        Idle = uint.MaxValue,

        Login = 0x1000,
        HeartBeat,

        BotStart = 0x2000,
        VerifySliderCaptcha,
        VerifyImageCaptcha,
        VerifySmsCaptcha,
        PrivateMessage,
        GroupMessage,

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
