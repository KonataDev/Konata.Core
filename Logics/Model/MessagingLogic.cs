using System;
using System.Threading.Tasks;

using Konata.Core.Events;
using Konata.Core.Message;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageNotifyEvent))]

    [BusinessLogic("Messaging Logic", "Responsible for the core messages.")]
    public class MessagingLogic : BaseLogic
    {
        private static string TAG = "Messaging Logic";

        internal MessagingLogic(BusinessComponent context)
            : base(context) { }

        public override void Incoming(ProtocolEvent e)
        {
            switch (e)
            {
                // Pull new private message
                case PrivateMessageNotifyEvent pull:
                    PrivateMessagePull();
                    return;

                // Received a private message
                case PrivateMessageEvent priv:
                    break;

                // Received a group message
                case GroupMessageEvent group:
                    ConfirmReadGroupMessage(group);
                    break;
            }

            // Forward messages to userend
            Context.PostEventToEntity(e);

            // TODO:
            // Update Group list cache or friend list cache
        }

        public Task<int> SendPrivateMessage(uint friendUin, MessageChain message)
        {
            // TODO: 
            // Handle the message chain
            // Figure out the image chains then upload them
            throw new NotImplementedException();
        }

        public Task<int> SendGroupMessage(uint groupUin, MessageChain message)
        {
            // TODO: 
            // Handle the message chain
            // Figure out the image chains then upload them
            throw new NotImplementedException();
        }

        internal void ConfirmReadGroupMessage(GroupMessageEvent e)
        {
            Context.PostEvent<PacketComponent>(new GroupMessageReadEvent
            {
                GroupUin = e.GroupUin,
                RequestId = e.MessageId,
                SessionSequence = e.SessionSequence,
            });
        }

        internal void PrivateMessagePull()
        {
            Context.PostEvent<PacketComponent>(new PrivateMessagePullEvent
            {
                SyncCookie = Context.GetComponent<ConfigComponent>().KeyStore.Account.SyncCookie
            });
        }

        internal void UpdateSyncCookie(PrivateMessageEvent privateMessage)
            => Context.GetComponent<ConfigComponent>().SyncCookie(privateMessage.SyncCookie);
    }
}
