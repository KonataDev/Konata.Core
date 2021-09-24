using Konata.Core.Events;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupPokeEvent))]
    [EventSubscribe(typeof(GroupMessageRecallEvent))]
    [EventSubscribe(typeof(GroupMuteMemberEvent))]
    [EventSubscribe(typeof(GroupSettingsAnonymousEvent))]
    [EventSubscribe(typeof(PushConfigEvent))]
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

            switch (e)
            {
                // Handle push config
                case PushConfigEvent push:
                    OnPushConfig(push);
                    break;

                // Just forward messages to userend
                default:
                    Context.PostEventToEntity(e);
                    break;
            }
        }

        /// <summary>
        /// Push config
        /// </summary>
        /// <param name="e"></param>
        private void OnPushConfig(PushConfigEvent e)
        {
            // Update the config
            ConfigComponent.HighwayConfig.Host = e.HighwayHost;
            ConfigComponent.HighwayConfig.Port = e.HighwayPort;
            ConfigComponent.HighwayConfig.Ticket = e.HighwayToken;

            Context.LogI(TAG, "Highway server has changed" +
                              $" to {e.HighwayHost}:{e.HighwayPort}");
        }
    }
}
