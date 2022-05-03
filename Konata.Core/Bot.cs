using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Components;
using Konata.Core.Entity;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
[GenerateEvents(
    // Others
    typeof(LogEvent), typeof(BotOnlineEvent), typeof(BotOfflineEvent), typeof(CaptchaEvent),
    // Group events
    typeof(GroupMessageEvent), typeof(GroupMuteMemberEvent), typeof(GroupPokeEvent), typeof(GroupKickMemberEvent), typeof(GroupMemberIncreaseEvent), typeof(GroupPromoteAdminEvent), typeof(GroupMessageRecallEvent), typeof(GroupInviteEvent), typeof(GroupRequestJoinEvent),
    // Friend events
    typeof(FriendMessageEvent), typeof(FriendPokeEvent), typeof(FriendMessageRecallEvent), typeof(FriendTypingEvent), typeof(FriendRequestEvent))]
[GenerateComponents("GetComponent", typeof(BusinessComponent), typeof(ConfigComponent), typeof(PacketComponent), typeof(ScheduleComponent), typeof(SocketComponent), typeof(HighwayComponent))]
public partial class Bot : BaseEntity, IDisposable
{
    /// <summary>
    /// Create a bot
    /// </summary>
    /// <param name="config"><b>[In]</b> Bot configuration</param>
    /// <param name="device"><b>[In]</b> Bot device definition</param>
    /// <param name="keystore"><b>[In]</b> Bot keystore</param>
    [Obsolete("Use BotFather.Create() instead.")]
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

    public void Dispose() => UnloadComponents();

    #region Bot Information

    /// <summary>
    /// Uin
    /// </summary>
    public uint Uin => KeyStore.Account.Uin;

    /// <summary>
    /// Name
    /// </summary>
    public string Name => KeyStore.Account.Name;

    /// <summary>
    /// Keystore
    /// </summary>
    public BotKeyStore KeyStore => ConfigComponent.KeyStore;

    #endregion

    #region Default Handlers

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
