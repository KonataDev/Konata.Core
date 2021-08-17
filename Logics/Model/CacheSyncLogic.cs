using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(OnlineStatusEvent))]
    [EventSubscribe(typeof(GroupMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageNotifyEvent))]
    [BusinessLogic("Cache Sync Logic", "Sync the cache such as grouplist or friendlist and more.")]
    public class CacheSyncLogic : BaseLogic
    {
        private const string TAG = "CacheSyncLogic";

        internal CacheSyncLogic(BusinessComponent context)
            : base(context)
        {
        }

        public override void Incoming(ProtocolEvent e)
        {
            switch (e)
            {
                // Bot is online
                case OnlineStatusEvent {EventType: OnlineStatusEvent.Type.Online}:
                    Task.Run(SyncGroupList);
                    Task.Run(SyncFriendList);
                    return;

                // New group message coming
                case GroupMessageEvent group:
                    SyncMemberInfo(group);
                    return;

                // private message coming
                case PrivateMessageEvent friend:
                    SyncFriendInfo(friend);
                    return;
            }
        }

        /// <summary>
        /// Sync group list
        /// </summary>
        private async void SyncGroupList()
        {
            var result = await Context.PostEvent<PacketComponent, PullGroupListEvent>
                (new PullGroupListEvent {SelfUin = Context.Bot.Uin});

            // Request successfully
            if (result.ResultCode == 0)
            {
                // Sync group list
                foreach (var i in result.GroupInfo)
                {
                    ConfigComponent.TouchGroupInfo(i);
                }
            }

            Context.LogI(TAG, $"Group list sync finished with code {result.ResultCode}, " +
                              $"Total {result.GroupInfo.Count} groups.");
        }

        /// <summary>
        /// Sync friend list cache
        /// </summary>
        private void SyncFriendList()
        {
            // TODO:
            // Sync friend list cache
        }

        private void SyncMemberInfo(GroupMessageEvent e)
        {
            ConfigComponent.TouchGroupMemberInfo
                (e.GroupUin, e.MemberUin, e.MemberCard);
        }

        private void SyncFriendInfo(PrivateMessageEvent e)
        {
            // TODO:
            // Sync friend cache
        }
    }
}
