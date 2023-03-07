using System;
using System.Collections.Generic;
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

// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core;

/// <summary>
/// Bot instance
/// </summary>
public class Bot : BaseEntity, IDisposable
{
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
        var component = GetComponent<ConfigComponent>();
        component.Initial(keystore, config, device);

        // Setup event handlers
        InitializeHandlers();

        // Start components
        StartAllComponents();
    }

    public void Dispose()
    {
        UnloadComponents();
        
    }

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

    internal TaskScheduler Scheduler;
    
    internal BusinessComponent BusinessComponent
        => GetComponent<BusinessComponent>();

    internal ConfigComponent ConfigComponent
        => GetComponent<ConfigComponent>();

    internal PacketComponent PacketComponent
        => GetComponent<PacketComponent>();

    internal SocketComponent SocketComponent
        => GetComponent<SocketComponent>();

    /// <summary>
    /// Keystore
    /// </summary>
    public BotKeyStore KeyStore
        => ConfigComponent.KeyStore;

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
}
