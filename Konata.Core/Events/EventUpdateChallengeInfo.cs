using System;

namespace Konata.Events
{
    public class EventUpdateChallengeInfo : EventParacel
    {
        public string Session { get; set; }

        public string SmsPhone { get; set; }

        public string SmsToken { get; set; }
    }
}
