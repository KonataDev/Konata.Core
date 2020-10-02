using System;
using Konata.Msf;
using Konata.Msf.Crypto;
using Konata.Msf.Network;
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
        internal uint _uin;
        internal string _password;

        internal Bot _bot;
        internal SsoMan _ssoMan;
        internal KeyRing _keyRing;
        internal OicqStatus _oicqStatus;

        public Core(Bot bot, uint uin, string password)
        {
            _uin = uin;
            _password = password;

            _bot = bot;
            _ssoMan = new SsoMan(this);
            _keyRing = new KeyRing(uin, password);
        }

        #region Core Methods

        /// <summary>
        /// 連接到伺服器
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return _ssoMan.Connect();
        }

        /// <summary>
        /// 斷開連接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            return _ssoMan.Disconnect();
        }

        /// <summary>
        /// 請求登錄
        /// </summary>
        /// <returns></returns>
        public bool WtLoginTgtgt()
        {
            return Service.Run(this, "Wtlogin.Login", "Request_TGTGT");
        }

        /// <summary>
        /// 提交滑塊驗證碼
        /// </summary>
        /// <param name="sigSission"></param>
        /// <param name="sigTicket"></param>
        /// <returns></returns>
        public bool WtLoginCheckSlider(string sigSission, string sigTicket)
        {
            return Service.Run(this, "Wtlogin.Login", "Request_SliderCaptcha",
                sigSission, sigTicket);
        }

        #endregion

        #region Event Metods

        /// <summary>
        /// 發送用戶事件. 該事件會被派發到用戶層
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void PostUserEvent(EventType type, params object[] args)
        {
            _bot.PostUserEvent(type, args);
        }

        /// <summary>
        /// 發送用戶事件. 該事件會被派發到用戶層
        /// </summary>
        /// <param name="type"></param>
        public void PostUserEvent(EventType type)
        {
            _bot.PostUserEvent(type, null);
        }

        /// <summary>
        /// 發送系統事件. 該事件會在系統内部響應
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void PostSystemEvent(EventType type, params object[] args)
        {
            _bot.PostSystemEvent(type, args);
        }

        /// <summary>
        /// 發送系統事件. 該事件會在系統内部響應
        /// </summary>
        /// <param name="type"></param>
        public void PostSystemEvent(EventType type)
        {
            _bot.PostSystemEvent(type, null);
        }

        #endregion 
    }
}
