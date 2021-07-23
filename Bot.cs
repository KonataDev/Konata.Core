using System;
using System.Threading.Tasks;

using Konata.Core.Events;
using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;
using Konata.Core.Attributes;

namespace Konata.Core
{
    public partial class Bot : BaseEntity
    {
        /// <summary>
        /// Create a bot
        /// </summary>
        /// <param name="config"><b>[In] </b>bot configuration</param>
        /// <param name="device"><b>[In] </b>bot device definition</param>
        /// <param name="keystore"><b>[In] </b>bot keystore</param>
        /// <param name="listener"><b>[In] </b>bot event handler</param>
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
                component.LoadKeyStore(keystore, device.Model.IMEI);
            }
        }

        #region Bot Information

        public uint Uin
            => KeyStore.Account.Uin;

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

        public BotKeyStore KeyStore
            => ConfigComponent.KeyStore;

        #endregion

        #region Protocol Methods

        /// <summary>
        /// Login
        /// </summary>
        public Task<bool> Login()
            => BusinessComponent.Login();

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public Task<bool> Logout()
            => BusinessComponent.Logout();

        /// <summary>
        /// Submit Slider ticket
        /// </summary>
        /// <param name="ticket"><b>[In]</b> Slider ticket</param>
        public void SubmitSliderTicket(string ticket)
            => BusinessComponent.SubmitSliderTicket(ticket);

        /// <summary>
        /// Submit SMS code.
        /// </summary>
        /// <param name="code"><b>[In]</b> SMS code</param>
        public void SubmitSMSCode(string code)
            => BusinessComponent.SubmitSMSCode(code);

        /// <summary>
        /// Kick a member in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
        public Task<GroupKickMemberEvent> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
            => BusinessComponent.GroupKickMember(groupUin, memberUin, preventRequest);

        /// <summary>
        /// Mute a member in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="timeSeconds"><b>[In]</b> Mute time. </param>
        public Task<GroupMuteMemberEvent> GroupMuteMember(uint groupUin, uint memberUin, uint timeSeconds)
            => BusinessComponent.GroupMuteMember(groupUin, memberUin, timeSeconds);

        /// <summary>
        /// Promote a member to admin in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="toggleAdmin"><b>[In]</b> Flag to toggle set or unset. </param>
        public Task<GroupPromoteAdminEvent> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
            => BusinessComponent.GroupPromoteAdmin(groupUin, memberUin, toggleAdmin);

        /// <summary>
        /// Send group message
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin.</param>
        /// <param name="message"><b>[In]</b> Message chain to be send.</param>
        /// <returns></returns>
        public Task<GroupMessageEvent> SendGroupMessage(uint groupUin, MessageChain message)
            => BusinessComponent.SendGroupMessage(groupUin, message);

        /// <summary>
        /// Send friend message
        /// </summary>
        /// <param name="friendUin"><b>[In]</b> Friend uin.</param>
        /// <param name="message"><b>[In]</b> Message chain to be send.</param>
        /// <returns></returns>
        public Task<PrivateMessageEvent> SendPrivateMessage(uint friendUin, MessageChain message)
            => BusinessComponent.SendPrivateMessage(friendUin, message);

        /// <summary>
        /// Get online status
        /// </summary>
        /// <returns></returns>
        public OnlineStatusEvent.Type GetOnlineStatus()
            => BusinessComponent.GetOnlineStatus();

        /// <summary>
        /// Set online status
        /// </summary>
        /// <param name="status"><b>[In]</b> Online status</param>
        /// <returns></returns>
        public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
            => BusinessComponent.SetOnlineStatus(status);

        /// <summary>
        /// Is Online
        /// </summary>
        /// <returns></returns>
        public bool IsOnline()
            => GetOnlineStatus() != OnlineStatusEvent.Type.Offline;
        #endregion
    }
}
