using System;
using Konata.Packets;
using Konata.Packets.Sso;

namespace Konata.Events
{
    public class EventSsoMessage : EventParacel
    {
        public SsoMessage PayloadMsg { get; set; }

        public RequestFlag RequestFlag { get; set; }

        public string Command { get; set; }
    }

    public class EventDraftSsoMessage : EventParacel
    {
        public uint Sequence { get; set; }

        public uint Session { get; set; }
    }

    public class EventDropServiceSsoSeq : EventParacel
    {
        public string ServiceName { get; set; }
    }
}
