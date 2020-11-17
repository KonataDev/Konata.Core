using System;
using Konata.Packets;

namespace Konata.Events
{
    public class EventServiceMessage : EventParacel
    {
        public ServiceMessage PayloadMsg { get; set; }
    }
}
