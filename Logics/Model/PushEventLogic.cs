using Konata.Core.Events;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupPokeEvent))]
    [EventSubscribe(typeof(GroupMessageRecallEvent))]
    [EventSubscribe(typeof(GroupMuteMemberEvent))]
    [EventSubscribe(typeof(GroupSettingsAnonymousEvent))]
    [BusinessLogic("PushEvent Logic", "Forward push events to userend.")]
    public class PushEventLogic : BaseLogic
    {
        private const string TAG = "PushEvent Logic";

        internal PushEventLogic(BusinessComponent context)
            : base(context)
        {
        }

        public override void Incoming(ProtocolEvent e)
        {
            // TODO:
            // Confirm the push events with server
            
            // Just forward messages to userend
            Context.PostEventToEntity(e);
        }
    }
}
