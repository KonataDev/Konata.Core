using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;
using Konata.Core.Exceptions;
using Konata.Core.Exceptions.Model;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core
{
    /// <summary>
    /// Bot instance
    /// </summary>
    public partial class Bot : BaseEntity
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
            => BusinessComponent.WtExchange.Login();

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
        /// Send the message to a given group
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin. </param>
        /// <param name="builder"><b>[In]</b> Message chain builder. </param>
        /// <returns>Return true for operation successfully.</returns>
        /// <exception cref="MessagingException"></exception>
        public Task<bool> SendGroupMessage(uint groupUin, MessageBuilder builder)
            => BusinessComponent.Messaging.SendGroupMessage(groupUin, builder.Build());

        /// <summary>
        /// Send the message to a given friend
        /// </summary>
        /// <param name="friendUin"><b>[In]</b> Friend uin. </param>
        /// <param name="builder"><b>[In]</b> Message chain builder. </param>
        /// <returns>Return true for operation successfully.</returns>
        /// <exception cref="MessagingException"></exception>
        public Task<bool> SendPrivateMessage(uint friendUin, MessageBuilder builder)
            => BusinessComponent.Messaging.SendPrivateMessage(friendUin, builder.Build());

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
    }
}
