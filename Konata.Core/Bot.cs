using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Konata.Core.Entity;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Components;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Utils;

[assembly: InternalsVisibleTo("Konata.Core.Test")]
[assembly: InternalsVisibleTo("Konata.Framework")]

// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Konata.Core;

/// <summary>
/// Bot instance
/// </summary>
public class Bot : BaseEntity, IDisposable
{
    #region Bot Information

    /// <summary>
    /// Uin
    /// </summary>
    public uint Uin
        => KeyStore.Account.Uin;

    /// <summary>
    /// Name
    /// </summary>
    public string Name
        => KeyStore.Account.Name;

    internal readonly TaskScheduler Scheduler;

    /// <summary>
    /// App info
    /// </summary>
    internal AppInfo AppInfo { get; }

    /// <summary>
    /// Keystore
    /// </summary>
    public BotKeyStore KeyStore { get; }

    /// <summary>
    /// Device info
    /// </summary>
    internal BotDevice DeviceInfo { get; }

    /// <summary>
    /// Global config
    /// </summary>
    internal BotConfig GlobalConfig { get; }

    internal byte[] SyncCookie
    {
        set => KeyStore.Account.SyncCookie = value;
        get => KeyStore.Account.SyncCookie;
    }

    /// <summary>
    /// Highway server
    /// </summary>
    internal Highway HighwayServer { get; set; }

    private readonly ConcurrentDictionary<uint, BotGroup> _groupList;
    private readonly ConcurrentDictionary<uint, BotFriend> _friendList;

    /// <summary>
    /// Create a bot
    /// </summary>
    /// <param name="config"><b>[In]</b> Bot configuration</param>
    /// <param name="device"><b>[In]</b> Bot device definition</param>
    /// <param name="keystore"><b>[In]</b> Bot keystore</param>
    [Obsolete("Use BotFather.Create instead.")]
    public Bot(BotConfig config, BotDevice device, BotKeyStore keystore)
    {
        // Init task scheduler
        Scheduler = new TaskScheduler();

        // Load components
        LoadComponents<ComponentAttribute>();

        // Setup configs
        KeyStore = keystore;
        GlobalConfig = config;
        DeviceInfo = device;

        // Initialize keystore
        KeyStore.Initial(config, device);

        // Select protocol type
        AppInfo = config.Protocol switch
        {
            OicqProtocol.AndroidPhone => AppInfo.AndroidPhone,
            OicqProtocol.Watch => AppInfo.Watch,
            OicqProtocol.Ipad => AppInfo.Ipad,
            OicqProtocol.AndroidPad => AppInfo.AndroidPad,
            _ => AppInfo.AndroidPhone
        };

        if (GlobalConfig.HighwayChunkSize is <= 1024 or > 1048576)
        {
            // LogW(TAG, $"The valid range of '{nameof(GlobalConfig.HighwayChunkSize)}'" +
            //           $"is from 1024 to 1048576 bytes, but current is {GlobalConfig.HighwayChunkSize}. Force reset to 8192.");
            GlobalConfig.HighwayChunkSize = 8192;
        }

        if (GlobalConfig.DefaultTimeout <= 2000)
        {
            // LogW(TAG, "The timeout you configured is less than 2000ms, " +
            //           "this can cause server communication chances to fail. Force reset to 6000.");
            GlobalConfig.DefaultTimeout = 6000;
        }

        // var component = GetComponent<ConfigComponent>();
        // component.Initial(keystore, config, device);

        // Setup event handlers
        InitializeHandlers();

        // Start components
        StartAllComponents();

        _groupList = new();
        _friendList = new();
    }

    public void Dispose()
    {
        UnloadComponents();
        Scheduler.Dispose();
    }

    internal BusinessComponent BusinessComponent
        => GetComponent<BusinessComponent>();

    internal PacketComponent PacketComponent
        => GetComponent<PacketComponent>();

    internal SocketComponent SocketComponent
        => GetComponent<SocketComponent>();

    #endregion

    #region Default Handlers

    public delegate void KonataEvent<in TArgs>(Bot sender, TArgs args);

    /// <summary>
    /// Handle log event
    /// </summary>
    public event KonataEvent<LogEvent> OnLog;

    /// <summary>
    /// Handle bot online event
    /// </summary>
    public event KonataEvent<BotOnlineEvent> OnBotOnline;

    /// <summary>
    /// Handle bot offline event
    /// </summary>
    public event KonataEvent<BotOfflineEvent> OnBotOffline;

    /// <summary>
    /// Handle captcha event
    /// </summary>
    public event KonataEvent<CaptchaEvent> OnCaptcha;

    /// <summary>
    /// On group message event
    /// </summary>
    public event KonataEvent<GroupMessageEvent> OnGroupMessage;

    /// <summary>
    /// On group mute event
    /// </summary>
    public event KonataEvent<GroupMuteMemberEvent> OnGroupMute;

    /// <summary>
    /// On group recall message event
    /// </summary>
    public event KonataEvent<GroupMessageRecallEvent> OnGroupMessageRecall;

    /// <summary>
    /// On group poke event
    /// </summary>
    public event KonataEvent<GroupPokeEvent> OnGroupPoke;

    /// <summary>
    /// On group member decrease event
    /// </summary>
    public event KonataEvent<GroupKickMemberEvent> OnGroupMemberDecrease;

    /// <summary>
    /// On group member increase event
    /// </summary>
    public event KonataEvent<GroupMemberIncreaseEvent> OnGroupMemberIncrease;

    /// <summary>
    /// On group admin set/unset event
    /// </summary>
    public event KonataEvent<GroupPromoteAdminEvent> OnGroupPromoteAdmin;

    /// <summary>
    /// On group invite event
    /// </summary>
    public event KonataEvent<GroupInviteEvent> OnGroupInvite;

    /// <summary>
    /// On group request join event
    /// </summary>
    public event KonataEvent<GroupRequestJoinEvent> OnGroupRequestJoin;

    /// <summary>
    /// On friend message event
    /// </summary>
    public event KonataEvent<FriendMessageEvent> OnFriendMessage;

    /// <summary>
    /// On group recall message event
    /// </summary>
    public event KonataEvent<FriendMessageRecallEvent> OnFriendMessageRecall;

    /// <summary>
    /// On friend poke event
    /// </summary>
    public event KonataEvent<FriendPokeEvent> OnFriendPoke;

    /// <summary>
    /// On friend typing event
    /// </summary>
    public event KonataEvent<FriendTypingEvent> OnFriendTyping;

    /// <summary>
    /// On friend request event
    /// </summary>
    public event KonataEvent<FriendRequestEvent> OnFriendRequest;

    private Dictionary<Type, Action<BaseEvent>> _dict;

    /// <summary>
    /// Handlers initialization
    /// </summary>
    internal void InitializeHandlers()
    {
        _dict = new()
        {
            // Other
            {typeof(LogEvent), e => OnLog?.Invoke(this, (LogEvent) e)},
            {typeof(CaptchaEvent), e => OnCaptcha?.Invoke(this, (CaptchaEvent) e)},
            {typeof(BotOnlineEvent), e => OnBotOnline?.Invoke(this, (BotOnlineEvent) e)},
            {typeof(BotOfflineEvent), e => OnBotOffline?.Invoke(this, (BotOfflineEvent) e)},

            // Group events
            {typeof(GroupMessageEvent), e => OnGroupMessage?.Invoke(this, (GroupMessageEvent) e)},
            {typeof(GroupMuteMemberEvent), e => OnGroupMute?.Invoke(this, (GroupMuteMemberEvent) e)},
            {typeof(GroupPokeEvent), e => OnGroupPoke?.Invoke(this, (GroupPokeEvent) e)},
            {typeof(GroupKickMemberEvent), e => OnGroupMemberDecrease?.Invoke(this, (GroupKickMemberEvent) e)},
            {typeof(GroupMemberIncreaseEvent), e => OnGroupMemberIncrease?.Invoke(this, (GroupMemberIncreaseEvent) e)},
            {typeof(GroupPromoteAdminEvent), e => OnGroupPromoteAdmin?.Invoke(this, (GroupPromoteAdminEvent) e)},
            {typeof(GroupMessageRecallEvent), e => OnGroupMessageRecall?.Invoke(this, (GroupMessageRecallEvent) e)},
            {typeof(GroupInviteEvent), e => OnGroupInvite?.Invoke(this, (GroupInviteEvent) e)},
            {typeof(GroupRequestJoinEvent), e => OnGroupRequestJoin?.Invoke(this, (GroupRequestJoinEvent) e)},

            // Friend events
            {typeof(FriendMessageEvent), e => OnFriendMessage?.Invoke(this, (FriendMessageEvent) e)},
            {typeof(FriendPokeEvent), e => OnFriendPoke?.Invoke(this, (FriendPokeEvent) e)},
            {typeof(FriendMessageRecallEvent), e => OnFriendMessageRecall?.Invoke(this, (FriendMessageRecallEvent) e)},
            {typeof(FriendTypingEvent), e => OnFriendTyping?.Invoke(this, (FriendTypingEvent) e)},
            {typeof(FriendRequestEvent), e => OnFriendRequest?.Invoke(this, (FriendRequestEvent) e)}
        };
    }

    /// <summary>
    /// Post event to user end
    /// </summary>
    /// <param name="anyEvent"></param>
    internal override void PostEventToEntity(BaseEvent anyEvent)
    {
        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                // Call user event
                _dict[anyEvent.GetType()].Invoke(anyEvent);
            }
            catch (Exception e)
            {
                // Suppress exceptions
                OnLog?.Invoke(this, LogEvent.Create("Bot",
                    LogLevel.Exception, $"{e.StackTrace}\n{e.Message}"));
            }
        });
    }

    /// <summary>
    /// Retrieve the handler is registered
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    internal bool HandlerRegistered<TEvent>()
        where TEvent : BaseEvent => _dict[typeof(TEvent)] != null;

    #endregion

    /// <summary>
    /// Get member information
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="memberUin"><b>[In]</b> Member uin</param>
    /// <param name="memberInfo"><b>[Out]</b> Member information</param>
    internal bool TryGetMemberInfo(uint groupUin,
        uint memberUin, out BotMember memberInfo)
    {
        // Get member information
        memberInfo = null;
        return _groupList.TryGetValue(groupUin, out var groupInfo)
               && groupInfo.Members.TryGetValue(memberUin, out memberInfo);
    }

    /// <summary>
    /// Get member information
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="memberUin"><b>[In]</b> Member uin</param>
    internal BotMember GetMemberInfo(uint groupUin, uint memberUin)
        => _groupList[groupUin].Members[memberUin];

    /// <summary>
    /// Get group information
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="groupInfo"><b>[Out]</b> Group information</param>
    /// <returns></returns>
    internal bool TryGetGroupInfo(uint groupUin, out BotGroup groupInfo)
        => _groupList.TryGetValue(groupUin, out groupInfo);

    /// <summary>
    /// Get group information
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <returns></returns>
    internal BotGroup GetGroupInfo(uint groupUin)
        => _groupList[groupUin];

    /// <summary>
    /// Get friend information
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin</param>
    /// <param name="friendInfo"><b>[Out]</b> Friend information</param>
    /// <returns></returns>
    internal bool TryGetFriendInfo(uint friendUin, out BotFriend friendInfo)
        => _friendList.TryGetValue(friendUin, out friendInfo);

    /// <summary>
    /// Get friend information
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin</param>
    /// <returns></returns>
    internal BotFriend GetFriendInfo(uint friendUin)
        => _friendList[friendUin];

    /// <summary>
    /// Add or update group
    /// </summary>
    /// <param name="groupInfo"></param>
    /// <returns></returns>
    internal BotGroup TouchGroupInfo(BotGroup groupInfo)
    {
        // Touch if the group does not exist 
        if (!_groupList.TryGetValue(groupInfo.Uin, out var group))
        {
            // Add the group
            group = groupInfo;
            _groupList.TryAdd(groupInfo.Uin, groupInfo);
        }

        else
        {
            // Update group information
            group.Name = groupInfo.Name;
            group.MemberCount = groupInfo.MemberCount;
            group.MaxMemberCount = groupInfo.MaxMemberCount;
            group.Muted = groupInfo.Muted;
            group.MutedMe = groupInfo.MutedMe;
            group.LastUpdate = DateTime.Now;
        }

        return group;
    }

    /// <summary>
    /// Add or update group information
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="groupName"><b>[In]</b> Group name</param>
    internal BotGroup TouchGroupInfo(uint groupUin, string groupName = null)
    {
        // Touch if the group does not exist 
        if (!_groupList.TryGetValue(groupUin, out var group))
        {
            group = new BotGroup
            {
                Uin = groupUin,
                Name = groupName,
            };

            // Touch the group
            _groupList.TryAdd(groupUin, group);
        }

        else
        {
            // Update group name
            if (groupName?.Length > 0
                && groupName != group.Name)
            {
                group.Name = groupName;
            }
        }

        return group;
    }

    /// <summary>
    /// Add or update group member
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="memberInfo">><b>[In]</b> Member info</param>
    /// <returns></returns>
    internal BotMember TouchGroupMemberInfo(uint groupUin, BotMember memberInfo)
    {
        // Touch if the group does not exist 
        if (!TryGetMemberInfo(groupUin, memberInfo.Uin, out var member))
        {
            // Add the member
            member = memberInfo;
            _groupList[groupUin].Members.Add(member.Uin, member);
        }

        else
        {
            // Update member information
            member.Age = memberInfo.Age;
            member.Name = memberInfo.Name;
            member.NickName = memberInfo.NickName;
            member.Gender = memberInfo.Gender;
            member.Level = memberInfo.Level;
            member.FaceId = memberInfo.FaceId;
            member.IsAdmin = memberInfo.IsAdmin;
            member.MuteTimestamp = memberInfo.MuteTimestamp;
            member.LastSpeakTime = memberInfo.LastSpeakTime;
            member.SpecialTitle = memberInfo.SpecialTitle;
            member.SpecialTitleExpiredTime = memberInfo.SpecialTitleExpiredTime;
            member.JoinTime = memberInfo.JoinTime;
        }

        // Update member role
        if (_groupList[groupUin].OwnerUin == member.Uin)
            member.Role = RoleType.Owner;

        // Group admin
        else if (member.IsAdmin)
            member.Role = RoleType.Admin;

        // Normal member
        else
            member.Role = RoleType.Member;

        return member;
    }

    /// <summary>
    /// Touch group member info
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="memberUin"><b>[In]</b> Member uin</param>
    /// <param name="memberCardName"><b>[In]</b> Member card name</param>
    internal BotMember TouchGroupMemberInfo(uint groupUin,
        uint memberUin, string memberCardName)
    {
        // Touch the group first
        var group = TouchGroupInfo(groupUin);

        // Update cache
        if (!group.Members.TryGetValue
                (memberUin, out var member))
        {
            member = new BotMember
            {
                Uin = memberUin,
                Name = memberCardName,
                NickName = memberCardName,
            };

            // Touch the member
            group.Members.Add(memberUin, member);
        }

        else
        {
            // Update member card name
            if (memberCardName.Length > 0
                && memberCardName != group.Name)
            {
                member.NickName = memberCardName;
            }
        }

        return member;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal BotFriend TouchFriendInfo(BotFriend friendInfo)
    {
        // Touch if the group does not exist 
        if (!TryGetFriendInfo(friendInfo.Uin, out var friend))
        {
            // Add the member
            friend = friendInfo;
            _friendList.TryAdd(friendInfo.Uin, friendInfo);
        }

        else
        {
            // Update friend information
            friend.Name = friendInfo.Name;
            friend.Gender = friendInfo.Gender;
            friend.FaceId = friendInfo.FaceId;
        }

        return friend;
    }

    /// <summary>
    /// Check if lacks the member cache
    /// </summary>
    /// <returns></returns>
    internal bool IsLackMemberCacheForGroup(uint groupUin)
    {
        // Check the cache
        if (TryGetGroupInfo(groupUin, out var group))
        {
            return group.MemberCount != group.Members.Count;
        }

        // Group not existed
        return false;
    }

    /// <summary>
    /// Get group uin from a code
    /// </summary>
    /// <param name="groupCode"></param>
    /// <returns></returns>
    internal ulong GetGroupUin(uint groupCode)
        => _groupList.FirstOrDefault(x => x.Value.Code == groupCode).Key;

    /// <summary>
    /// Get group code from an uin
    /// </summary>
    /// <param name="groupUin"></param>
    /// <returns></returns>
    internal ulong GetGroupCode(uint groupUin)
        => TryGetGroupInfo(groupUin, out var group) ? group.Code : 0;

    /// <summary>
    /// Get group list
    /// </summary>
    /// <returns></returns>
    internal IReadOnlyList<BotGroup> GetGroupList()
        => _groupList.Values.ToList();

    /// <summary>
    /// Get group member list
    /// </summary>
    /// <param name="groupUin"></param>
    /// <returns></returns>
    internal IReadOnlyList<BotMember> GetGroupMemberList(uint groupUin)
        => _groupList[groupUin].Members.Values.ToList();

    /// <summary>
    /// Get friend list
    /// </summary>
    /// <returns></returns>
    internal IReadOnlyList<BotFriend> GetFriendList()
        => _friendList.Values.ToList();
}
