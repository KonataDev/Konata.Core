using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;

// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(OnlineStatusEvent))]
[EventSubscribe(typeof(GroupMessageEvent))]
[EventSubscribe(typeof(FriendMessageEvent))]
[BusinessLogic("Cache Sync Logic", "Sync the cache such as grouplist or friendlist and more.")]
internal class CacheSyncLogic : BaseLogic
{
    private const string TAG = "CacheSyncLogic";

    internal CacheSyncLogic(BusinessComponent context)
        : base(context)
    {
    }

    public override async Task Incoming(ProtocolEvent e)
    {
        switch (e)
        {
            // Bot is online
            case OnlineStatusEvent {EventType: OnlineStatusEvent.Type.Online}:
                await Task.WhenAll(SyncGroupList(), SyncFriendList(), SyncGuildInfo());
                return;

            // New group message coming
            case GroupMessageEvent group:
                SyncMemberInfo(group);
                return;

            // private message coming
            case FriendMessageEvent friend:
                SyncFriendInfo(friend);
                return;
        }
    }

    /// <summary>
    /// Sync group list
    /// </summary>
    internal async Task<bool> SyncGroupList()
    {
        var result = await PullGroupList(Context);
        {
            // Request successfully
            if (result.ResultCode == 0)
            {
                // Sync group list
                foreach (var i in result.GroupInfo)
                {
                    Context.Bot.TouchGroupInfo(i);
                }
            }
        }

        Context.LogI(TAG, $"Group list sync finished, " +
                          $"Total {result.GroupInfo.Count} groups.");
        return true;
    }

    /// <summary>
    /// Sync group member list cache
    /// </summary>
    /// <param name="groupUin"></param>
    internal async Task<bool> SyncGroupMemberList(uint groupUin)
    {
        var nextUin = 0U;
        var memberCount = 0;

        if (!Context.Bot.TryGetGroupInfo
                (groupUin, out var groupInfo) || groupInfo.Code == 0)
        {
            Context.LogE(TAG, $"Sync group member failed, " +
                              $"no such group {groupUin}:{groupInfo.Code}");
            return false;
        }

        do
        {
            // Pull group member list
            var result = await PullGroupMemberList
                (Context, groupUin, groupInfo.Code, nextUin);

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
                Context.Bot.TouchGroupMemberInfo(groupUin, i);
            }

            nextUin = result.NextUin;
            memberCount += result.MemberInfo.Count;

            Context.LogI(TAG, $"Group [{groupUin}] now syncing " +
                              $"[{memberCount}/{groupInfo.MemberCount}]");

            // Continue
        } while (nextUin != 0);

        Context.LogI(TAG, $"Group [{groupUin}] sync finished, " +
                          $"Total {memberCount} members.");

        return true;
    }

    /// <summary>
    /// Sync friend list cache
    /// </summary>
    internal async Task<bool> SyncFriendList()
    {
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
                Context.Bot.TouchFriendInfo(i);
            }

            nextIndex += limit;
            friendCount += result.FriendInfo.Count;
            if (friendCount >= result.TotalFriendCount)
            {
                nextIndex = 0;
            }

            // Continue
        } while (nextIndex != 0);

        Context.LogI(TAG, $"Friend list sync finished, " +
                          $"Total {friendCount} friends.");

        return true;
    }

    private async Task SyncGuildInfo()
    {
        await PullGroupMemberList(Context);
    }

    /// <summary>
    /// Sync member info while the message coming
    /// </summary>
    /// <param name="e"></param>
    private void SyncMemberInfo(GroupMessageEvent e)
    {
        Context.Bot.TouchGroupMemberInfo
            (e.GroupUin, e.MemberUin, e.MemberCard);
    }

    /// <summary>
    /// Sync friend info while the message coming
    /// </summary>
    /// <param name="e"></param>
    private void SyncFriendInfo(FriendMessageEvent e)
    {
        // TODO:
        // Sync friend cache
    }

    /// <summary>
    /// Get group list
    /// </summary>
    /// <param name="forceUpdate"></param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public async Task<IReadOnlyList<BotGroup>> GetGroupList(bool forceUpdate)
    {
        try
        {
            if (forceUpdate) await SyncGroupList();
            return Context.Bot.GetGroupList();
        }

        catch (Exception)
        {
            throw new SyncFailedException(-1,
                "Failed to sync the group list.");
        }
    }

    /// <summary>
    /// Get group list
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="forceUpdate"></param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public async Task<IReadOnlyList<BotMember>> GetGroupMemberList(uint groupUin, bool forceUpdate = false)
    {
        try
        {
            if (forceUpdate ||
                Context.Bot.IsLackMemberCacheForGroup(groupUin))
            {
                await SyncGroupList();
                await SyncGroupMemberList(groupUin);
            }

            return Context.Bot.GetGroupMemberList(groupUin);
        }

        catch (Exception)
        {
            throw new SyncFailedException(-1,
                "Failed to sync the group list.");
        }
    }

    /// <summary>
    /// Get group member info
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="memberUin"></param>
    /// <param name="forceUpdate"></param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public async Task<BotMember> GetGroupMemberInfo
        (uint groupUin, uint memberUin, bool forceUpdate)
    {
        try
        {
            if (forceUpdate ||
                Context.Bot.IsLackMemberCacheForGroup(groupUin))
            {
                await SyncGroupList();
                await SyncGroupMemberList(groupUin);
            }

            return Context.Bot.GetMemberInfo(groupUin, memberUin);
        }

        catch (Exception)
        {
            throw new SyncFailedException(-1,
                "Failed to sync the group or group member list.");
        }
    }

    /// <summary>
    /// Get friend list
    /// </summary>
    /// <param name="forceUpdate"></param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public async Task<IReadOnlyList<BotFriend>> GetFriendList(bool forceUpdate)
    {
        try
        {
            if (forceUpdate) await SyncFriendList();
            return Context.Bot.GetFriendList();
        }

        catch (Exception)
        {
            throw new SyncFailedException(-1,
                "Failed to sync the friend list.");
        }
    }

    /// <summary>
    /// Get csrf token
    /// </summary>
    /// <returns></returns>
    public Task<string> GetCsrfToken()
    {
        var token = 5381;
        var stkey = Context.Bot.KeyStore.Session.StKey;

        // Calculate token
        foreach (var i in stkey)
        {
            token += (token << 5) + i;
        }

        // And 0x7FFFFFFF
        token &= int.MaxValue;

        return Task.FromResult(token.ToString());
    }

    #region Stub methods

    private static Task<PullGroupListEvent> PullGroupList(BusinessComponent context)
        => context.SendPacket<PullGroupListEvent>(PullGroupListEvent.Create(context.Bot.Uin));

    private static Task<PullFriendListEvent> PullFriendList(BusinessComponent context, uint startIndex, uint limitNum)
        => context.SendPacket<PullFriendListEvent>(PullFriendListEvent.Create(context.Bot.Uin, startIndex, limitNum));

    private static Task<PullGroupMemberListEvent> PullGroupMemberList(BusinessComponent context, uint groupUin, ulong groupCode, uint nextUin)
        => context.SendPacket<PullGroupMemberListEvent>(PullGroupMemberListEvent.Create(context.Bot.Uin, groupUin, groupCode, nextUin));

    private static Task<GuildSyncFirstView> PullGroupMemberList(BusinessComponent context)
        => context.SendPacket<GuildSyncFirstView>(GuildSyncFirstView.Create());

    #endregion
}
