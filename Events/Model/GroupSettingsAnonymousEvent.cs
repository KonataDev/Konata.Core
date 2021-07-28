using System;

namespace Konata.Core.Events.Model
{
    public class GroupSettingsAnonymousEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint OperatorUin { get; set; }

        public bool ToggleType { get; set; }
    }
}
