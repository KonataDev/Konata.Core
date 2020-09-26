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

        public bool Connect()
        {
            return _ssoMan.Initialize();
        }

        public bool WtLoginTgtgt()
        {
            return Service.Run(this, "Wtlogin.Login", "Request_TGTGT");
        }

        public bool WtLoginCheckSlider(string sigSission, string sigTicket)
        {
            return Service.Run(this, "Wtlogin.Login", "Request_SliderCaptcha",
                sigSission, sigTicket);
        }

        public void PostUserEvent(EventType type, params object[] args)
        {
            _bot.PostUserEvent(type, args);
        }

        public void PostSystemEvent(EventType type, params object[] args)
        {
            _bot.PostSystemEvent(type, args);
        }
        
        public void PostUserEvent(EventType type)
        {
            _bot.PostUserEvent(type, null);
        }
        
        public void PostSystemEvent(EventType type)
        {
            _bot.PostSystemEvent(type, null);
        }
    }
}
