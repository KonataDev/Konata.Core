using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;
using Konata.Core.Attributes;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global
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
            var result = await PullGroupList(Context);
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
        /// Sync group member list cache
        /// </summary>
        /// <param name="groupUin"></param>
        internal async Task<bool> SyncGroupMemberList(uint groupUin)
        {
            var nextUin = 0U;
            var memberCount = 0;

            if (!ConfigComponent.TryGetGroupInfo
                (groupUin, out var groupInfo) || groupInfo.Code == 0)
            {
                Context.LogE(TAG, $"Sync group member " +
                                  $"failed, no such group {groupUin}");
                return false;
            }

            do
            {
                // Pull group member list
                var result = await PullGroupMemberList
                    (Context, nextUin, groupInfo.Code, nextUin);

                // Check if failed 
                if (result.ResultCode != 0)
                {
                    Context.LogE(TAG, $"Sync group member failed " +
                                      $"with an error code {result.ErrorCode}");
                    return false;
                }

                // Sync the member list
                foreach (var i in result.MemberInfo)
                {
                    ConfigComponent.TouchGroupMemberInfo(groupUin, i);
                }

                nextUin = result.NextUin;
                memberCount += result.MemberInfo.Count;

                Context.LogI(TAG, $"Group [{groupUin}] now syncing " +
                                  $"[{memberCount}/{groupInfo.MemberCount}]");

                // Continue
            } while (nextUin != 0);

            Context.LogI(TAG, $"Group [{groupUin}] syncing finished, " +
                              $"Total {memberCount} members.");

            return true;
        }

        /// <summary>
        /// Sync friend list cache
        /// </summary>
        internal async Task<bool> SyncFriendList()
        {
            // TODO:
            // Sync friend list cache

            const int limit = 150;
            var nextIndex = 0U;
            var friendCount = 0;

            do
            {
                // Pull group member list
                var result = await PullFriendList(Context, nextIndex, limit);

                // Check if failed 
                if (result.ResultCode != 0)
                {
                    Context.LogE(TAG, $"Sync friend list failed " +
                                      $"with an error code {result.ErrorCode}");
                    return false;
                }

                // Sync the friend list
                foreach (var i in result.FriendInfo)
                {
                    ConfigComponent.TouchFriendInfo(i);
                }

                nextIndex += limit;
                friendCount += result.FriendInfo.Count;
                if (friendCount >= result.TotalFriendCount)
                {
                    nextIndex = 0;
                }

                // Continue
            } while (nextIndex != 0);

            Context.LogI(TAG, $"Friend syncing finished, " +
                              $"Total {friendCount} friends.");

            return true;
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

        private static Task<PullGroupListEvent> PullGroupList(BusinessComponent context)
            => context.PostPacket<PullGroupListEvent>(PullGroupListEvent.Create(context.Bot.Uin));

        private static Task<PullFriendListEvent> PullFriendList(BusinessComponent context, uint startIndex, uint limitNum)
            => context.PostPacket<PullFriendListEvent>(PullFriendListEvent.Create(context.Bot.Uin, startIndex, limitNum));

        private static Task<PullGroupMemberListEvent> PullGroupMemberList(BusinessComponent context, uint groupUin, ulong groupCode, uint nextUin)
            => context.PostPacket<PullGroupMemberListEvent>(PullGroupMemberListEvent.Create(context.Bot.Uin, groupUin, groupCode, nextUin));

        #endregion
    }
}
