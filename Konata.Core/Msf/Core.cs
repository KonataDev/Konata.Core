using System;
using Konata.Msf.Packets.Oicq;

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

    public class Core
    {
        public Bot Bot { get; private set; }

        public SsoMan SsoMan { get; private set; }

        public UserSigInfo SigInfo { get; private set; }

        public Core(Bot bot, uint uin, string password)
        {
            Bot = bot;
            SsoMan = new SsoMan(this);
            SigInfo = new UserSigInfo(uin, password);
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

        #endregion

        #region Event Methods

        /// <summary>
        /// 發送用戶事件. 該事件會被派發到用戶層
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void PostUserEvent(EventType type, params object[] args)
        {
            Bot.PostUserEvent(type, args);
        }

        /// <summary>
        /// 發送用戶事件. 該事件會被派發到用戶層
        /// </summary>
        /// <param name="type"></param>
        public void PostUserEvent(EventType type)
        {
            Bot.PostUserEvent(type, null);
        }

        /// <summary>
        /// 發送系統事件. 該事件會在系統内部響應
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void PostSystemEvent(EventType type, params object[] args)
        {
            Bot.PostSystemEvent(type, args);
        }

        /// <summary>
        /// 發送系統事件. 該事件會在系統内部響應
        /// </summary>
        /// <param name="type"></param>
        public void PostSystemEvent(EventType type)
        {
            Bot.PostSystemEvent(type, null);
        }

        #endregion
    }
}
