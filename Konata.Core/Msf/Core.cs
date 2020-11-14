using System;
using Konata.Events;

namespace Konata.Msf
{
    //   Mobileqq Service Framework

    //  ++----> SsoMan <---> PacketMan
    //  ||         +-------+
    //  ||                 |    
    //  ||	 +-> Msf.Core  |   
    //  ||	 |      |      |   
    //  ||   ++  Service <-+
    //  ||	  |   /    \
    //  ++--- WtLogin OnlinePush...etc 
    //   +----------------+

    public class Core : EventComponent
    {
        public SsoMan SsoMan { get; private set; }

        public UserSigInfo SigInfo { get; private set; }

        public Core(EventPumper eventPumper)
            : base(eventPumper)
        {
            eventHandlers += OnWtLogin;

        }

        private EventParacel OnWtLogin(EventParacel eventParacel)
        {
            if (eventParacel is EventLogin)
                return PostEvent<Services.Wtlogin.Login>(null);

            return EventParacel.Reject;
        }

        #region Core Methods

        /// <summary>
        /// 連接到伺服器
        /// </summary>
        /// <returns></returns>
        public bool Connect() =>
            SsoMan.Connect();

        /// <summary>
        /// 斷開連接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() =>
            SsoMan.DisConnect();

        /// <summary>
        /// 請求登錄
        /// </summary>
        /// <returns></returns>
        public bool WtLoginTgtgt() =>
             Service.Run(this, "Wtlogin.Login", "Request_TGTGT");

        /// <summary>
        /// 提交滑塊驗證碼
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public bool WtLoginCheckSlider(string ticket) =>
            Service.Run(this, "Wtlogin.Login", "Request_SliderCaptcha", ticket);

        /// <summary>
        /// 提交SMS驗證碼
        /// </summary>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public bool WtLoginCheckSms(string smsCode) =>
            Service.Run(this, "Wtlogin.Login", "Request_SmsCaptcha", smsCode);

        /// <summary>
        /// 刷新SMS驗證碼
        /// </summary>
        /// <returns></returns>
        public bool WtLoginRefreshSms() =>
            Service.Run(this, "Wtlogin.Login", "Request_RefreshSms");

        /// <summary>
        /// 注冊客戶端
        /// </summary>
        /// <returns></returns>
        public bool StatSvc_RegisterClient() =>
            Service.Run(this, "StatSvc.register");

        /// <summary>
        /// 獲取在綫狀態
        /// </summary>
        /// <returns></returns>
        public bool StatSvc_GetOnlineStatus() =>
            Service.Run(this, "StatSvc.GetOnlineStatus");

        /// <summary>
        /// 發送心跳
        /// </summary>
        /// <returns></returns>
        public bool Heartbeat_Alive() =>
            Service.Run(this, "Heartbeat.Alive");

        /// <summary>
        /// 批量移除群成員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="preventRequest"></param>
        /// <returns></returns>
        public bool OidbSvc_0x8a0_0(uint groupUin, uint[] memberUin, bool preventRequest) =>
            Service.Run(this, "OidbSvc.0x8a0_0", groupUin, memberUin, preventRequest);

        /// <summary>
        /// 移除單個群成員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="preventRequest"></param>
        /// <returns></returns>
        public bool OidbSvc_0x8a0_1(uint groupUin, uint memberUin, bool preventRequest) =>
            Service.Run(this, "OidbSvc.0x8a0_1", groupUin, memberUin, preventRequest);

        /// <summary>
        /// 設置群管理員
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="promoteAdmin"></param>
        /// <returns></returns>
        public bool OidbSvc_0x55c_1(uint groupUin, uint memberUin, bool promoteAdmin) =>
            Service.Run(this, "OidbSvc.0x55c_1", groupUin, memberUin, promoteAdmin);

        /// <summary>
        /// 設置群禁言
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="timeSeconds"></param>
        /// <returns></returns>
        public bool OidbSvc_0x570_8(uint groupUin, uint memberUin, uint timeSeconds) =>
            Service.Run(this, "OidbSvc.0x570_8", groupUin, memberUin, timeSeconds);

        /// <summary>
        /// 設置群成員頭銜
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="specialTitle"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public bool OidbSvc_0x8fc_2(uint groupUin, uint memberUin,
            string specialTitle, uint? expiredTime) =>
            Service.Run(this, "OidbSvc.0x8fc_2", groupUin, memberUin, specialTitle, expiredTime);

        /// <summary>
        /// 拉取好友列表
        /// </summary>
        /// <param name="selfUin"></param>
        /// <returns></returns>
        public bool GetTroopListReqV2(uint selfUin) =>
            Service.Run(this, "friendlist.GetTroopListReqV2", selfUin);

        #endregion
    }

    public class EventLogin : EventNotify
    {
        // Do Nothing
    }

    public class EventCaptchaCtl : EventParacel
    {
        public enum CtlType
        {
            Sms,
            Image,
            Slider,
            // Face
        }

        public CtlType Type { get; set; }

        /// <summary>
        /// Captcha results
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// For CtlType.Slider
        /// </summary>
        public string SliderUrl { get; set; }

        /// <summary>
        /// For CtlType.Sms
        /// </summary>
        public string SmsPhoneNumber { get; set; }

        /// <summary>
        /// For CtlType.Sms
        /// </summary>
        public string SmsPhoneCountryCode { get; set; }
    }

    public class EventGroupCtl : EventParacel
    {
        public enum CtlType
        {
            KickMember,
            MuteMember,
            PromoteAdmin,
            SetSpecialTitle,
            SetGroupCard,
        }

        public CtlType Type { get; set; }

        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// For PromoteAdmin or KickMember <br/>
        /// <b>KickMember</b>: Block the member <br/>
        /// <b>PromoteAdmin</b>: Set or Unset
        /// </summary>
        public bool ToggleType { get; set; }

        /// <summary>
        /// For MuteMember or SetSpecialTitle <br/>
        /// <b>MuteMember</b>: Mute time <br/>
        /// <b>SetSpecialTitle</b>: Title expired time
        /// </summary>
        public uint? TimeSeconds { get; set; }

        /// <summary>
        /// For SetSpecialTitle
        /// </summary>
        public string SpecialTitle { get; set; }

        /// <summary>
        /// For SetGroupCard
        /// </summary>
        public string GroupCard { get; set; }
    }

    public class EventGroupCtlRsp : EventParacel
    {
        public bool Success { get; set; }

        public int ResultCode { get; set; }
    }

    public class EventStatSvc : EventParacel
    {

    }

    public class EventLoginFailed : EventParacel
    {
        public string Reason { get; set; }
    }
}
