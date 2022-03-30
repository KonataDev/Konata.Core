using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Konata.Core.Entity;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Components;
using Konata.Core.Events;
using Konata.Core.Events.Model;

[assembly: InternalsVisibleToAttribute("Konata.Core.Test")]
[assembly: InternalsVisibleToAttribute("Konata.Framework")]

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

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
        // Load components
        LoadComponents<ComponentAttribute>();

        // Setup configs
        var component = GetComponent<ConfigComponent>();
        {
            component.LoadConfig(config);
            component.LoadDeviceInfo(device);
            component.LoadKeyStore(keystore, device.Model.Imei);
        }

        // Setup event handlers
        InitializeHandlers();

        // Start components
        StartAllComponents();
    }

    public void Dispose()
        => UnloadComponents();

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

    internal BusinessComponent BusinessComponent
        => GetComponent<BusinessComponent>();

    internal ConfigComponent ConfigComponent
        => GetComponent<ConfigComponent>();

    internal PacketComponent PacketComponent
        => GetComponent<PacketComponent>();

    internal ScheduleComponent ScheduleComponent
        => GetComponent<ScheduleComponent>();

    internal SocketComponent SocketComponent
        => GetComponent<SocketComponent>();

    internal HighwayComponent HighwayComponent
        => GetComponent<HighwayComponent>();

    /// <summary>
    /// Keystore
    /// </summary>
    public BotKeyStore KeyStore
        => ConfigComponent.KeyStore;

    #endregion

    #region Default Handlers

    /// <summary>
    /// Handle log event
    /// </summary>
    public event EventHandler<LogEvent> OnLog;

    /// <summary>
    /// Handle captcha event
    /// </summary>
    public event EventHandler<CaptchaEvent> OnCaptcha;

    /// <summary>
    /// On online status changed event
    /// </summary>
    [Obsolete("This event will be removed in future.")]
    public event EventHandler<OnlineStatusEvent> OnOnlineStatusChanged;

    /// <summary>
    /// On group message event
    /// </summary>
    public event EventHandler<GroupMessageEvent> OnGroupMessage;

    /// <summary>
    /// On group mute event
    /// </summary>
    public event EventHandler<GroupMuteMemberEvent> OnGroupMute;

    /// <summary>
    /// On group recall message event
    /// </summary>
    public event EventHandler<GroupMessageRecallEvent> OnGroupMessageRecall;

    /// <summary>
    /// On group poke event
    /// </summary>
    public event EventHandler<GroupPokeEvent> OnGroupPoke;

    /// <summary>
    /// On group member decrease event
    /// </summary>
    public event EventHandler<GroupKickMemberEvent> OnGroupKickMember;

    /// <summary>
    /// On group admin set/unset event
    /// </summary>
    public event EventHandler<GroupPromoteAdminEvent> OnGroupPromoteAdmin;

    /// <summary>
    /// On friend message event
    /// </summary>
    public event EventHandler<FriendMessageEvent> OnFriendMessage;

    /// <summary>
    /// On group recall message event
    /// </summary>
    public event EventHandler<FriendMessageRecallEvent> OnFriendMessageRecall;

    /// <summary>
    /// On friend poke event
    /// </summary>
    public event EventHandler<FriendPokeEvent> OnFriendPoke;

    /// <summary>
    /// On friend typing event
    /// </summary>
    public event EventHandler<FriendTypingEvent> OnFriendTyping;

    private Dictionary<Type, Action<BaseEvent>> _dict;

    /// <summary>
    /// Handlers initialization
    /// </summary>
    internal void InitializeHandlers()
    {
        _dict = new()
        {
            {typeof(LogEvent), e => OnLog?.Invoke(this, (LogEvent) e)},
            {typeof(CaptchaEvent), e => OnCaptcha?.Invoke(this, (CaptchaEvent) e)},
            {typeof(OnlineStatusEvent), e => OnOnlineStatusChanged?.Invoke(this, (OnlineStatusEvent) e)},
            {typeof(GroupMessageEvent), e => OnGroupMessage?.Invoke(this, (GroupMessageEvent) e)},
            {typeof(GroupMuteMemberEvent), e => OnGroupMute?.Invoke(this, (GroupMuteMemberEvent) e)},
            {typeof(GroupPokeEvent), e => OnGroupPoke?.Invoke(this, (GroupPokeEvent) e)},
            {typeof(GroupKickMemberEvent), e => OnGroupKickMember?.Invoke(this, (GroupKickMemberEvent) e)},
            {typeof(GroupPromoteAdminEvent), e => OnGroupPromoteAdmin?.Invoke(this, (GroupPromoteAdminEvent) e)},
            {typeof(GroupMessageRecallEvent), e => OnGroupMessageRecall?.Invoke(this, (GroupMessageRecallEvent) e)},
            {typeof(FriendMessageEvent), e => OnFriendMessage?.Invoke(this, (FriendMessageEvent) e)},
            {typeof(FriendPokeEvent), e => OnFriendPoke?.Invoke(this, (FriendPokeEvent) e)},
            {typeof(FriendMessageRecallEvent), e => OnFriendMessageRecall?.Invoke(this, (FriendMessageRecallEvent) e)},
            {typeof(FriendTypingEvent), e => OnFriendTyping?.Invoke(this, (FriendTypingEvent) e)},
        };

        // Default group message handler
        OnGroupMessage += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group]{e.GroupUin} " +
                                  $"[Member]{e.MemberUin} {e.Chain}"));
        };

        // Default group mute handler
        OnGroupMute += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group Mute]{e.GroupUin} " +
                                  $"[Operator]{e.OperatorUin} " +
                                  $"[Member]{e.MemberUin} " +
                                  $"[Time]{e.TimeSeconds} sec."));
        };

        // Default group poke handler
        OnGroupPoke += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group Poke]{e.GroupUin} " +
                                  $"[Operator]{e.OperatorUin} " +
                                  $"[Member]{e.MemberUin}"));
        };

        // Default group recall handler
        OnGroupMessageRecall += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group Recall]{e.GroupUin} " +
                                  $"[Member]{e.OperatorUin}"));
        };

        // Default group promote handler
        OnGroupPromoteAdmin += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group Promote]{e.GroupUin} " +
                                  $"[Member]{e.MemberUin} " +
                                  $"[Set]{e.ToggleType}"));
        };

        // Default group promote handler
        OnGroupKickMember += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Group Kick]{e.GroupUin} " +
                                  $"[Operator]{e.OperatorUin} " +
                                  $"[Member]{e.MemberUin}"));
        };

        // Default friend message handler
        OnFriendMessage += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Friend]{e.FriendUin} {e.Chain}"));
        };

        // Default friend message recall handler
        OnFriendMessageRecall += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Friend Recall]{e.FriendUin}"));
        };

        // Default friend poke handler
        OnFriendPoke += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Friend Poke]{e.FriendUin}"));
        };

        // Default friend poke handler
        OnFriendTyping += (sender, e) =>
        {
            OnLog?.Invoke(sender, LogEvent.Create("Bot",
                LogLevel.Verbose, $"[Friend Typing]{e.FriendUin}"));
        };
    }

    /// <summary>
    /// Post event to user end
    /// </summary>
    /// <param name="anyEvent"></param>
    internal override void PostEventToEntity(BaseEvent anyEvent)
    {
        Task.Run(() =>
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
