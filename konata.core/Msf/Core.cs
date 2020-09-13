using System;
using konata.Msf;
using Konata.Msf.Crypto;
using Konata.Msf.Network;

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

    public delegate void EventDelegate(uint signal);

    public class Core
    {
        internal uint _uin;
        internal string _password;

        internal uint _lastError;
        internal string _lastErrorStr;

        internal SsoMan _ssoMan;
        internal KeyRing _keyRing;
        internal EventDelegate _eventHandler;

        public Core(uint uin, string password)
        {
            _uin = uin;
            _password = password;

            _lastError = 0;
            _lastErrorStr = "";

            _ssoMan = new SsoMan(this);
            _keyRing = new KeyRing();
        }

        public bool Connect()
        {
            return _ssoMan.Initialize();
        }

        public bool RegisterDelegate(EventDelegate func)
        {
            _eventHandler = func;
            return true;
        }

        public bool DoLogin()
        {
            return ServiceRoutine.Run(this, "Wtlogin.Login", "Request_TGTGT");
        }

        internal void EmitError(uint errcode, string errstr)
        {
            if (_lastError == 0)
            {
                _lastError = errcode;
                _lastErrorStr = errstr;
                SendEvent(1);
            }
        }

        private void SendEvent(uint signal)
        {
            _eventHandler?.Invoke(signal);
        }

    }
}
