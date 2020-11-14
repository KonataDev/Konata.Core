using System;
using Konata.Events;
using Konata.Network;

namespace Konata
{
    public delegate bool UserEventProc(EventType e, params object[] a);

    public class Bot : EventPumper
    {
        private UserEventProc eventProc;

        public Bot(uint uin, string password)
        {
            RegisterComponent(new Core(this));
            RegisterComponent(new SsoMan(this));
            RegisterComponent(new PacketMan(this));
            RegisterComponent(new UserSigInfo(this, uin, password));
            RegisterComponent(new ToUser(this, eventProc));
        }

        /// <summary>
        /// 注冊事件委托回調方法
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool RegisterDelegate(UserEventProc callback)
        {
            if (eventProc != null)
            {
                return false;
            }

            eventProc = callback;
            return true;
        }

        #region Protocol Interoperation Methods

        /// <summary>
        /// 執行登錄
        /// </summary>
        public void Login()
            => PostEvent<Services.Wtlogin.Login>
            (new EventWtLoginExchange
            {
                Type = EventWtLoginExchange.EventType.Tgtgt
            });

        /// <summary>
        /// 提交滑塊驗證碼
        /// </summary>
        /// <param name="ticket"></param>
        public void SubmitSliderTicket(string ticket)
            => PostEvent<Services.Wtlogin.Login>
            (new EventWtLoginExchange
            {
                CaptchaResult = ticket,
                Type = EventWtLoginExchange.EventType.CheckSliderCaptcha,
            });

        /// <summary>
        /// 提交SMS驗證碼
        /// </summary>
        /// <param name="smsCode"></param>
        public void SubmitSmsCode(string smsCode)
            => PostEvent<Services.Wtlogin.Login>
            (new EventWtLoginExchange
            {
                CaptchaResult = smsCode,
                Type = EventWtLoginExchange.EventType.CheckSmsCaptcha
            });

        /// <summary>
        /// 移除群成員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="preventRequest"></param>
        public EventGroupCtlRsp KickGroupMember(uint groupUin,
            uint memberUin, bool preventRequest)
            => (EventGroupCtlRsp)CallEvent<Core>(new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = preventRequest,
                Type = EventGroupCtl.CtlType.KickMember
            });

        /// <summary>
        /// 設置群管理員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="toggleType"></param>
        public EventGroupCtlRsp PromoteGroupAdmin(uint groupUin,
            uint memberUin, bool toggleType)
            => (EventGroupCtlRsp)CallEvent<Core>(new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = toggleType,
                Type = EventGroupCtl.CtlType.PromoteAdmin
            });

        /// <summary>
        /// 設置群成員禁言
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="timeSeconds"></param>
        public EventGroupCtlRsp MuteGroupMember(uint groupUin,
            uint memberUin, uint timeSeconds)
            => (EventGroupCtlRsp)CallEvent<Core>(new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                TimeSeconds = timeSeconds,
                Type = EventGroupCtl.CtlType.MuteMember
            });

        /// <summary>
        /// 設置群成員頭銜
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="specialTitle"></param>
        /// <param name="expiredTime"></param>
        public EventGroupCtlRsp SetGroupMemberSpecialTitle(uint groupUin, uint memberUin,
            string specialTitle, uint? expiredTime)
            => (EventGroupCtlRsp)CallEvent<Core>(new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                SpecialTitle = specialTitle,
                TimeSeconds = expiredTime,
                Type = EventGroupCtl.CtlType.SetSpecialTitle
            });

        #endregion
    }

    public class ToUser : EventComponent
    {
        private readonly UserEventProc eventProc;

        public ToUser(EventPumper eventPumper, UserEventProc proc)
            : base(eventPumper)
        {
            eventProc = proc;
            eventHandlers += OnUserEvent;
        }

        private EventParacel OnUserEvent(EventParacel eventParacel)
        {
            eventProc(EventType.Idle, eventParacel);
            return EventParacel.Accept;
        }

        public class EventUserDoCaptcha : EventParacel
        {

        }
    }
}
