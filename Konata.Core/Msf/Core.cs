using System;
using System.Security.Cryptography;
using System.Text;
using Konata.Msf.Crypto;
using Konata.Msf.Packets.Oicq;
using Konata.Msf.Utils.Crypt;

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

        internal byte[] _gSecret;
        internal string _dPassword;
        internal string _smsPhone;

        internal Bot _bot;
        internal SsoMan _ssoMan;
        internal KeyRing _keyRing;
        internal OicqStatus _oicqStatus;

        public Core(Bot bot, uint uin, string password)
        {
            _uin = uin;
            _password = password;

            _dPassword = MakeDpassword();
            _gSecret = MakeGSecret(DeviceInfo.System.Imei, _dPassword, null);

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
            return _ssoMan.DisConnect();
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

        /// <summary>
        /// 提交SMS驗證碼
        /// </summary>
        /// <param name="sigSission"></param>
        /// <param name="sigSecret"></param>
        /// <param name="sigSmsCode"></param>
        /// <returns></returns>
        public bool WtLoginCheckSms(string sigSission, byte[] sigSecret, string sigSmsCode)
        {
            return Service.Run(this, "Wtlogin.Login", "Request_SmsCaptcha",
                sigSission, sigSecret, sigSmsCode, _gSecret);
        }

        /// <summary>
        /// 刷新SMS驗證碼
        /// </summary>
        /// <param name="sigSission"></param>
        /// <param name="sigSecret"></param>
        /// <returns></returns>
        public bool WtLoginRefreshSms(string sigSission, byte[] sigSecret)
        {
            return Service.Run(this, "Wtlogin.Login", "Request_RefreshSms", sigSission, sigSecret);
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

        internal byte[] MakeGSecret(string imei, string dpwd, byte[] salt)
        {
            var imeiByte = Encoding.UTF8.GetBytes(imei);
            var dpwdByte = Encoding.UTF8.GetBytes(dpwd);

            var buffer = new byte[imeiByte.Length + dpwdByte.Length +
                (salt != null ? salt.Length : 0)];
            return new Md5Cryptor().Encrypt(buffer);
        }

        internal string MakeDpassword()
        {
            try
            {
                var random = new Random();
                var seedTable = new byte[16];

                bool RandBoolean()
                {
                    return random.Next(0, 1) == 1;
                }

                using (RNGCryptoServiceProvider SecurityRandom =
                    new RNGCryptoServiceProvider())
                {
                    SecurityRandom.GetBytes(seedTable);
                }

                for (int i = 0; i < seedTable.Length; ++i)
                {
                    seedTable[i] = (byte)(Math.Abs(seedTable[i] % 26) + (RandBoolean() ? 97 : 65));
                }

                return Encoding.UTF8.GetString(seedTable);
            }
            catch
            {
                return "1234567890123456";
            }
        }
    }
}
