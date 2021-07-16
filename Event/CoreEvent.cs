using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Event
{
    public class CoreEvent : EventArgs
    {
        public Bot Bot { get; }

        public BaseEvent BaseEvent { get; }

        public CoreEvent(Bot bot, BaseEvent anyEvent)
        {
            Bot = bot;
            BaseEvent = anyEvent;
        }
    }
}
