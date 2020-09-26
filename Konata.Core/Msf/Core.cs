using System;
using System.Threading;
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

    //public delegate void EventDelegate(uint signal);

    public class Core
    {
        internal uint _uin;
        internal string _password;

        internal uint _lastError;
        internal string _lastErrorStr;

        internal Bot _bot;
        internal SsoMan _ssoMan;
        internal KeyRing _keyRing;
        internal OicqStatus _oicqStatus;

        public Core(Bot bot, uint uin, string password)
        {
            _uin = uin;
            _password = password;

            _lastError = 0;
            _lastErrorStr = "";

            _bot = bot;
            _ssoMan = new SsoMan(this);
            _keyRing = new KeyRing(uin, password);
        }

        public bool Connect()
        {
            return _ssoMan.Initialize();
        }

        public bool DoLogin()
        {
            return Service.Run(this, "Wtlogin.Login", "Request_TGTGT");
        }

        public bool DoVerifySliderCaptcha(string sigSission, string sigTicket)
        {
            return Service.Run(this, "Wtlogin.Login", "Request_SliderCaptcha",
                sigSission, sigTicket);
        }

        public void PostEvent(EventType type)
        {
            _bot.PostEvent(type);
        }

        public void PostEvent(EventType type, params object[] args)
        {
            _bot.PostEvent(type, args);
        }

    }
}
