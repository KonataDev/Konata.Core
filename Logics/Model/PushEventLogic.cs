using System;

using Konata.Core.Events;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupPokeEvent))]
    [EventSubscribe(typeof(GroupMessageRecallEvent))]
    [EventSubscribe(typeof(GroupMuteMemberEvent))]
    [EventSubscribe(typeof(GroupSettingsAnonymousEvent))]

    [BusinessLogic("Push Event Logic", "Forward push events to userend.")]
    public class PushEventLogic : BaseLogic
    {
        internal PushEventLogic(BusinessComponent context)
            : base(context) { }

        public override void Incoming(ProtocolEvent e)
        {
            // Just forward messages to userend
            Context.PostEventToEntity(e);
        }
    }
}
