using System;

using Konata.Core.Event;
using Konata.Core.Manager;
using Konata.Core.Service;
using Konata.Runtime.Base;

namespace Konata.Core
{
    public class Bot : Entity
    {
        public static async void Login(Bot bot)
            => ServiceManager.Instance.GetService<PacketService>()
                .SendDataToServer(new EventWtLogin
                {
                    Owner = bot,
                    EventType = EventWtLogin.Type.Tgtgt,
                });

        /// <summary>
        /// Submit Slider ticket
        /// </summary>
        /// <param name="bot"><b>[In]</b> The bot instance. </param>
        /// <param name="ticket"><b>[In]</b> Slider ticket</param>
        public static async void SubmitSliderTicket(Bot bot, string ticket)
            => ServiceManager.Instance.GetService<PacketService>()
                .SendDataToServer(new EventWtLogin
                {
                    Owner = bot,
                    WtLoginCaptchaResult = ticket,
                    EventType = EventWtLogin.Type.CheckSlider,
                });

        /// <summary>
        /// Submit SMS code.
        /// </summary>
        /// <param name="bot"><b>[In]</b> The bot instance. </param>
        /// <param name="code"><b>[In]</b> SMS code</param>
        public static async void SubmitSMSCode(Bot bot, string code)
            => ServiceManager.Instance.GetService<PacketService>()
                .SendDataToServer(new EventWtLogin
                {
                    Owner = bot,
                    WtLoginCaptchaResult = code,
                    EventType = EventWtLogin.Type.CheckSMS,
                });

        /// <summary>
        /// Kick a member in the specific group.
        /// </summary>
        /// <param name="bot"><b>[In]</b> The bot instance. </param>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
        public static async void GroupKickMember(Bot bot, uint groupUin,
            uint memberUin, bool preventRequest)
            => ServiceManager.Instance.GetService<PacketService>()
                .SendDataToServer(new EventGroupKickMember
                {
                    Owner = bot,
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = preventRequest
                });

        /// <summary>
        /// Promote a member to admin in the specific group.
        /// </summary>
        /// <param name="bot"><b>[In]</b> The bot instance. </param>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="toggleAdmin"><b>[In]</b> Flag to toggle set or unset. </param>
        public static async void GroupPromoteAdmin(Bot bot, uint groupUin,
            uint memberUin, bool toggleAdmin)
            => ServiceManager.Instance.GetService<PacketService>()
                .SendDataToServer(new EventGroupPromoteAdmin
                {
                    Owner = bot,
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = toggleAdmin
                });

        public static class BotFather
        {
            public static Bot Build(uint botUin, string botPassword)
                => LoadAllComponents(new Bot(), botUin, botPassword);

            private static Bot LoadAllComponents(Bot bot, uint botUin, string botPassword)
            {
                bot.AddComponent<EventComponent>();
                bot.AddComponent<ConfigManager>();
                bot.AddComponent<SsoInfoManager>();
                bot.AddComponent<UserSigManager>().InitializeProfile(botUin, botPassword);

                return bot;
            }
        }
    }
}
