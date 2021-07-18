using System;

using Konata.Core.Components.Model;

namespace Konata.Core.Components
{
    internal class InternalComponent : BaseComponent
    {
        public Bot Context
            => (Bot)Entity;

        public ConfigComponent ConfigComponent
            => Context.ConfigComponent;

        public BusinessComponent BusinessComponent
            => Context.BusinessComponent;

        public PacketComponent PacketComponent
            => Context.PacketComponent;

        public SocketComponent SocketComponent
            => Context.SocketComponent;

        public ScheduleComponent ScheduleComponent
            => Context.ScheduleComponent;

        internal InternalComponent()
            : base() { }
    }
}
