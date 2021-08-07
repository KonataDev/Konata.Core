using System;

using Konata.Core.Components.Model;

namespace Konata.Core.Components
{
    internal class InternalComponent : BaseComponent
    {
        public Bot Bot
            => (Bot)Entity;

        public ConfigComponent ConfigComponent
            => Bot.ConfigComponent;

        public BusinessComponent BusinessComponent
            => Bot.BusinessComponent;

        public PacketComponent PacketComponent
            => Bot.PacketComponent;

        public SocketComponent SocketComponent
            => Bot.SocketComponent;

        public ScheduleComponent ScheduleComponent
            => Bot.ScheduleComponent;

        public HighwayComponent HighwayComponent
            => Bot.HighwayComponent;

        internal InternalComponent()
            : base() { }
    }
}
