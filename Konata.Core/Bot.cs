using System;
using Konata.Events;
using Konata.Network;

namespace Konata
{
    // public delegate bool UserEventProc(EventType e, params object[] a);

    public class Bot : EventPumper
    {
        // private UserEventProc eventProc;

        public Bot(uint uin, string password)
        {
            RegisterComponent(new Core(this));
            RegisterComponent(new SsoMan(this));
            RegisterComponent(new PacketMan(this));
            RegisterComponent(new ServiceMan(this));
            RegisterComponent(new SigInfoMan(this, uin, password));
            RegisterComponent(new ToUser(this));
        }

        /// <summary>
        /// 注冊事件委托回調方法
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        //public bool RegisterDelegate(UserEventProc callback)
        //{
        //    if (eventProc != null)
        //    {
        //        return false;
        //    }

        //    eventProc = callback;
        //    return true;
        //}

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
        /// 移除單個群成員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="preventRequest"></param>
        public EventGroupCtlRsp KickGroupMember(uint groupUin,
            uint memberUin, bool preventRequest)
            => (EventGroupCtlRsp)CallEvent<Services.OidbSvc.Oidb0x8a0_1>
            (new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = preventRequest,
                Type = EventGroupCtl.EventType.KickMember
            });

        /// <summary>
        /// 批量移除群成員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="preventRequest"></param>
        public EventGroupCtlRsp KickGroupMembers(uint groupUin,
            uint[] membersUin, bool preventRequest)
            => (EventGroupCtlRsp)CallEvent<Services.OidbSvc.Oidb0x8a0_0>
            (new EventGroupCtl
            {
                GroupUin = groupUin,
                MembersUin = membersUin,
                ToggleType = preventRequest,
                Type = EventGroupCtl.EventType.KickMember
            });

        /// <summary>
        /// 設置群管理員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="toggleType"></param>
        public EventGroupCtlRsp PromoteGroupAdmin(uint groupUin,
            uint memberUin, bool toggleType)
            => (EventGroupCtlRsp)CallEvent<Services.OidbSvc.Oidb0x55c_1>
            (new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = toggleType,
                Type = EventGroupCtl.EventType.PromoteAdmin
            });

        /// <summary>
        /// 設置群成員禁言
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="timeSeconds"></param>
        public EventGroupCtlRsp MuteGroupMember(uint groupUin,
            uint memberUin, uint timeSeconds)
            => (EventGroupCtlRsp)CallEvent<Services.OidbSvc.Oidb0x570_8>
            (new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                TimeSeconds = timeSeconds,
                Type = EventGroupCtl.EventType.MuteMember
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
            => (EventGroupCtlRsp)CallEvent<Services.OidbSvc.Oidb0x8fc_2>
            (new EventGroupCtl
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                SpecialTitle = specialTitle,
                TimeSeconds = expiredTime,
                Type = EventGroupCtl.EventType.SetSpecialTitle
            });

        #endregion
    }

    public class ToUser : EventComponent
    {
        //private readonly UserEventProc eventProc;

        public ToUser(EventPumper eventPumper)
            : base(eventPumper)
        {
            // eventProc = proc;
            eventHandlers += OnUserEvent;
        }

        private EventParacel OnUserEvent(EventParacel eventParacel)
        {
            // eventProc(EventType.Idle, eventParacel);
            return EventParacel.Accept;
        }

        public class EventUserDoCaptcha : EventParacel
        {

        }
    }
}
