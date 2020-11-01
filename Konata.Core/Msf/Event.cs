using System;

namespace Konata.Msf
{
    public enum EventFilter : uint
    {
        System = 0x10000000,
        User = 0x20000000
    }

    public enum EventType : uint
    {
        Idle = 0x00,

        WtLogin = 0x1000,
        WtLoginOK,
        WtLoginVerifyDeviceLock,
        WtLoginVerifySliderCaptcha,
        WtLoginVerifyImageCaptcha,
        WtLoginVerifySmsCaptcha,
        WtLoginSendSms,
        HeartBeat,
        PbPushNotify,
        PbGetMsg,
        PbPushGroupMsg,
        KickMember,


        BotStart = 0x2000,
        LoginFailed,
        LoginSuccess,
        PrivateMessage,
        GroupMessage,
    }

    public class Event
    {
        public static readonly Event Idle =
            new Event(EventFilter.System, EventType.Idle);

        public EventType type;
        public EventFilter filter;
        public object[] args;

        public Event(EventFilter filter, EventType type,
            params object[] args)
        {
            this.type = type;
            this.filter = filter;
            this.args = args;
        }

        public Event(EventFilter filter, EventType type)
        {
            this.type = type;
            this.filter = filter;
        }
    }
}
