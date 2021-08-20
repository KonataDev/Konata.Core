using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Components.Model;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Components
{
    internal class InternalComponent : BaseComponent
    {
        public Bot Bot
            => (Bot) Entity;

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

        public async Task<TEvent> PostPacket<TEvent>(ProtocolEvent anyEvent, int timeout)
            where TEvent : ProtocolEvent
        {
            var task = timeout == 0
                ? Entity.PostEvent<PacketComponent>(anyEvent)
                : Entity.PostEvent<PacketComponent>(anyEvent, anyEvent.Timeout);
            return (TEvent) await task;
        }

        public Task<TEvent> PostPacket<TEvent>(ProtocolEvent anyEvent)
            where TEvent : ProtocolEvent => PostPacket<TEvent>(anyEvent, anyEvent.Timeout);

        public async void PostPacket(ProtocolEvent anyEvent)
        {
            var task = anyEvent.Timeout == 0
                ? Entity.PostEvent<PacketComponent>(anyEvent)
                : Entity.PostEvent<PacketComponent>(anyEvent, anyEvent.Timeout);
            await task;
        }
    }
}
