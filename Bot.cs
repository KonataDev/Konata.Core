using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Component;
using Konata.Core.Event.EventModel;

namespace Konata.Core
{
    public sealed class Bot : BaseEntity
    {
        internal Bot() { }

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
        public Task<GroupMessageEvent> SendGroupMessage(uint groupUin, List<MessageChain> message)
            => GetComponent<BusinessComponent>().SendGroupMessage(groupUin, message);

        /// <summary>
        /// Send friend message
        /// </summary>
        /// <param name="friendUin"><b>[In]</b> Friend uin.</param>
        /// <param name="message"><b>[In]</b> Message chain to be send.</param>
        /// <returns></returns>
        public Task<PrivateMessageEvent> SendPrivateMessage(uint friendUin, List<MessageChain> message)
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
