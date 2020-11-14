using System;

namespace Konata.Events
{
    public class EventUpdateSyncCookie : EventParacel
    {
        public byte[] SyncCookie { get; set; }
    }
}
