using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Components;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message.Model;

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
    public Bot(BotConfig config,
        BotDevice device, BotKeyStore keystore)
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

    #region Protocol Methods

    /// <summary>
    /// Bot login
    /// </summary>
    /// <returns></returns>
    public Task<bool> Login()
        => BusinessComponent.WtExchange.Login();

    /// <summary>
    /// Disconnect the socket and logout
    /// </summary>
    /// <returns></returns>
    public Task<bool> Logout()
        => BusinessComponent.WtExchange.Logout();

    /// <summary>
    /// Submit Slider ticket while login
    /// </summary>
    /// <param name="ticket"><b>[In]</b> Slider ticket</param>
    public bool SubmitSliderTicket(string ticket)
        => BusinessComponent.WtExchange.SubmitSliderTicket(ticket);

    /// <summary>
    /// Submit Sms code while login
    /// </summary>
    /// <param name="code"><b>[In]</b> Sms code</param>
    public bool SubmitSmsCode(string code)
        => BusinessComponent.WtExchange.SubmitSmsCode(code);

    /// <summary>
    /// Kick the member in a given group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public Task<bool> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
        => BusinessComponent.Operation.GroupKickMember(groupUin, memberUin, preventRequest);

    /// <summary>
    /// Mute the member in a given group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="timeSeconds"><b>[In]</b> Mute time. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public Task<bool> GroupMuteMember(uint groupUin, uint memberUin, uint timeSeconds)
        => BusinessComponent.Operation.GroupMuteMember(groupUin, memberUin, timeSeconds);

    /// <summary>
    /// Promote the member to admin in a given group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="toggleAdmin"><b>[In]</b> Flag to toggle set or unset. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public Task<bool> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
        => BusinessComponent.Operation.GroupPromoteAdmin(groupUin, memberUin, toggleAdmin);

    /// <summary>
    /// Set special title
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="specialTitle"><b>[In]</b> Special title. </param>
    /// <param name="expiredTime"><b>[In]</b> Exipred time. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public Task<bool> GroupSetSpecialTitle(uint groupUin, uint memberUin, string specialTitle, uint expiredTime)
        => BusinessComponent.Operation.GroupSetSpecialTitle(groupUin, memberUin, specialTitle, expiredTime);

    /// <summary>
    /// Leave group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public Task<bool> GroupLeave(uint groupUin)
        => BusinessComponent.Operation.GroupLeave(groupUin);

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="builder"><b>[In]</b> Message chain builder. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendGroupMessage(uint groupUin, MessageBuilder builder)
        => BusinessComponent.Messaging.SendGroupMessage(groupUin, builder.Build());

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="message"><b>[In]</b> Text message. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendGroupMessage(uint groupUin, string message)
        => SendGroupMessage(groupUin, new MessageBuilder(message));

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="chains"><b>[In]</b> Message chains. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendGroupMessage(uint groupUin, params BaseChain[] chains)
        => SendGroupMessage(groupUin, new MessageBuilder(chains));

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="builder"><b>[In]</b> Message chain builder. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendFriendMessage(uint friendUin, MessageBuilder builder)
        => BusinessComponent.Messaging.SendFriendMessage(friendUin, builder.Build());

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="message"><b>[In]</b> Text message. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendFriendMessage(uint friendUin, string message)
        => SendFriendMessage(friendUin, new MessageBuilder(message));

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
    /// <param name="chains"><b>[In]</b> Message chains. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    public Task<bool> SendFriendMessage(uint friendUin, params BaseChain[] chains)
        => SendFriendMessage(friendUin, new MessageBuilder(chains));

    /// <summary>
    /// Upload the image manually
    /// </summary>
    /// <param name="image"><b>[In]</b> Image to upload. </param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <returns></returns>
    public Task<bool> UploadGroupImage(ImageChain image, uint groupUin)
        => BusinessComponent.Messaging.UploadImage(image, groupUin, true);

    /// <summary>
    /// Get group list
    /// </summary>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public Task<IReadOnlyList<BotGroup>> GetGroupList(bool forceUpdate = false)
        => BusinessComponent.CacheSync.GetGroupList(forceUpdate);

    /// <summary>
    /// Get member member list
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public Task<IReadOnlyList<BotMember>> GetGroupMemberList(uint groupUin, bool forceUpdate = false)
        => BusinessComponent.CacheSync.GetGroupMemberList(groupUin, forceUpdate);

    /// <summary>
    /// Get friend list
    /// </summary>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public Task<IReadOnlyList<BotFriend>> GetFriendList(bool forceUpdate = false)
        => BusinessComponent.CacheSync.GetFriendList(forceUpdate);

    /// <summary>
    /// Get member info
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin. </param>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    public Task<BotMember> GetGroupMemberInfo(uint groupUin, uint memberUin, bool forceUpdate = false)
        => BusinessComponent.CacheSync.GetGroupMemberInfo(groupUin, memberUin, forceUpdate);

    /// <summary>
    /// Get csrf token <br/>
    /// </summary>
    /// <returns></returns>
    public Task<string> GetCsrfToken()
        => BusinessComponent.CacheSync.GetCsrfToken();

    /// <summary>
    /// Get online status
    /// </summary>
    /// <returns></returns>
    public OnlineStatusEvent.Type GetOnlineStatus()
        => BusinessComponent.WtExchange.OnlineType;

    /// <summary>
    /// Set online status
    /// </summary>
    /// <param name="status"><b>[In]</b> Online status</param>
    /// <returns></returns>
    public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
        => BusinessComponent.WtExchange.SetOnlineStatus(status);

    /// <summary>
    /// Is Online
    /// </summary>
    /// <returns></returns>
    public bool IsOnline()
        => GetOnlineStatus() != OnlineStatusEvent.Type.Offline;

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
    private void InitializeHandlers()
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
        ThreadPool.QueueUserWorkItem(_ =>
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
