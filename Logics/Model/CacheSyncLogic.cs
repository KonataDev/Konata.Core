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
        internal async void SyncGroupList()
        {
            var result = await PullGroupList(Context, Context.Bot.Uin);
            {
                // Request successfully
                if (result.ResultCode == 0)
                {
                    // Sync group list
                    foreach (var i in result.GroupInfo)
                    {
                        ConfigComponent.TouchGroupInfo(i);
                    }
                }
            }

            Context.LogI(TAG, $"Group list sync finished with code {result.ResultCode}, " +
                              $"Total {result.GroupInfo.Count} groups.");
        }

        /// <summary>
        /// Sync group member list
        /// </summary>
        /// <param name="groupUin"></param>
        internal async Task<bool> SyncGroupMemberList(uint groupUin)
        {
            var nextUin = 0U;
            var memberCount = 0;
            var groupCode = ConfigComponent.GetGroupCode(groupUin);

            // Convert group code failed
            if (groupCode == 0)
            {
                return false;
            }

            // Sync until finished
            do
            {
                // Pull group member list
                var result = await PullGroupMemberList
                    (Context, Context.Bot.Uin, nextUin, groupCode, nextUin);

                // Check if failed 
                if (result.ResultCode != 0)
                {
                    Context.LogE(TAG, $"Sync group member failed " +
                                      $"with an error code {result.ErrorCode}");
                    return false;
                }

                // Sync the member list
                foreach (var i in result.Members)
                {
                    ConfigComponent.TouchGroupMemberInfo(groupUin, i);
                }

                nextUin = result.NextUin;
                memberCount += result.Members.Count;

                // Continue
            } while (nextUin != 0);

            Context.LogI(TAG, $"Group [{groupUin}] sync finished, " +
                              $"Total {memberCount} members.");

            return true;
        }

        /// <summary>
        /// Sync friend list cache
        /// </summary>
        internal void SyncFriendList()
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

        #region Stub methods

        private static Task<PullGroupListEvent> PullGroupList(BusinessComponent context, uint selfUin)
            => context.PostEvent<PacketComponent, PullGroupListEvent>(new PullGroupListEvent {SelfUin = selfUin});

        private static Task<PullGroupMemberListEvent> PullGroupMemberList(BusinessComponent context, uint selfUin, uint groupUin, ulong groupCode, uint nextUin)
            => context.PostEvent<PacketComponent, PullGroupMemberListEvent>(new PullGroupMemberListEvent {SelfUin = selfUin, GroupUin = groupUin, GroupCode = groupCode, NextUin = nextUin});

        #endregion
    }
}
