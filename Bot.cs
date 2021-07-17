using System;
using System.Threading.Tasks;

using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Component;
using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Service;
using Konata.Utils;

namespace Konata.Core
{
    public class Bot : BaseEntity
    {
        /// <summary>
        /// Create a bot
        /// </summary>
        /// <param name="handler"><b>[In] </b>bot event handler</param>
        /// <param name="config"><b>[In] </b>bot configuration</param>
        /// <param name="device"><b>[In] </b>bot device definition</param>
        public Bot(BotConfig config, BotDevice device,
            BotKeyStore keystore, Action<CoreEvent> handler)
        {
            // Load components
            foreach (var type in Reflection
                .GetClassesByAttribute<ComponentAttribute>())
            {
                AddComponent((BaseComponent)Activator.CreateInstance(type));
            }

            // Setup event handler
            SetEventHandler(handler);

            // Setup configs
            var component = GetComponent<ConfigComponent>();
            {
                component.LoadConfig(config);
                component.LoadDeviceInfo(device);
                component.LoadSignInfo(new SignInfo(keystore));
            }
        }

        #region Bot Information

        public uint Uin
            => GetComponent<ConfigComponent>().SignInfo.Account.Uin;

        public string Name
            => GetComponent<ConfigComponent>().SignInfo.Account.Name;

        #endregion

        #region Protocol Methods

        /// <summary>
        /// Login
        /// </summary>
        public Task<bool> Login()
            => GetComponent<BusinessComponent>().Login();

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public Task<bool> Logout()
            => GetComponent<BusinessComponent>().Logout();

        /// <summary>
        /// Submit Slider ticket
        /// </summary>
        /// <param name="ticket"><b>[In]</b> Slider ticket</param>
        public void SubmitSliderTicket(string ticket)
            => GetComponent<BusinessComponent>().SubmitSliderTicket(ticket);

        /// <summary>
        /// Submit SMS code.
        /// </summary>
        /// <param name="code"><b>[In]</b> SMS code</param>
        public void SubmitSMSCode(string code)
            => GetComponent<BusinessComponent>().SubmitSMSCode(code);

        /// <summary>
        /// Kick a member in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
        public Task<GroupKickMemberEvent> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
            => GetComponent<BusinessComponent>().GroupKickMember(groupUin, memberUin, preventRequest);

        /// <summary>
        /// Mute a member in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="timeSeconds"><b>[In]</b> Mute time. </param>
        public Task<GroupMuteMemberEvent> GroupMuteMember(uint groupUin, uint memberUin, uint timeSeconds)
            => GetComponent<BusinessComponent>().GroupMuteMember(groupUin, memberUin, timeSeconds);

        /// <summary>
        /// Promote a member to admin in the specific group.
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="toggleAdmin"><b>[In]</b> Flag to toggle set or unset. </param>
        public Task<GroupPromoteAdminEvent> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
            => GetComponent<BusinessComponent>().GroupPromoteAdmin(groupUin, memberUin, toggleAdmin);

        /// <summary>
        /// Send group message
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin.</param>
        /// <param name="message"><b>[In]</b> Message chain to be send.</param>
        /// <returns></returns>
        public Task<GroupMessageEvent> SendGroupMessage(uint groupUin, MessageChain message)
            => GetComponent<BusinessComponent>().SendGroupMessage(groupUin, message);

        /// <summary>
        /// Send friend message
        /// </summary>
        /// <param name="friendUin"><b>[In]</b> Friend uin.</param>
        /// <param name="message"><b>[In]</b> Message chain to be send.</param>
        /// <returns></returns>
        public Task<PrivateMessageEvent> SendPrivateMessage(uint friendUin, MessageChain message)
            => GetComponent<BusinessComponent>().SendPrivateMessage(friendUin, message);

        /// <summary>
        /// Get online status
        /// </summary>
        /// <returns></returns>
        public OnlineStatusEvent.Type GetOnlineStatus()
            => GetComponent<BusinessComponent>().GetOnlineStatus();

        /// <summary>
        /// Set online status
        /// </summary>
        /// <param name="status"><b>[In]</b> Online status</param>
        /// <returns></returns>
        public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
            => GetComponent<BusinessComponent>().SetOnlineStatus(status);

        /// <summary>
        /// Is Online
        /// </summary>
        /// <returns></returns>
        public bool IsOnline()
            => GetOnlineStatus() != OnlineStatusEvent.Type.Offline;

        #endregion
    }
}
