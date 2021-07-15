using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Event
{
    public class CoreEvent : EventArgs
    {
        public uint Uin { get; }

        public BaseEvent BaseEvent { get; }

        public CoreEvent(uint uin, BaseEvent anyEvent)
        {
            Uin = uin;
            BaseEvent = anyEvent;
        }
    }
}
