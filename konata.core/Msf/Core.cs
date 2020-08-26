using System;
using System.Threading.Tasks;
using Konata.Msf.Network;

namespace Konata.Msf
{
    public delegate void EventDelegate(uint signal);

    public class Core
    {
        internal uint _uin;
        internal string _password;

        internal uint _lasterr;
        internal string _errstr;

        internal PacketMan _pakman;
        internal EventDelegate _handler;

        public Core(uint uin, string password)
        {
            _uin = uin;
            _password = password;

            _pakman = new PacketMan();

            _lasterr = 0;
            _errstr = "";
        }

        public bool Connect()
        {
            return _pakman.Init();
        }

        public bool RegisterDelegate(EventDelegate func)
        {
            _handler = func;
            return true;
        }

        public bool DoLogin()
        {
            return ServiceRoutine.Run("Wtlogin.Login", this);
        }

        internal void EmitError(uint errcode, string errstr)
        {
            if (_lasterr == 0)
            {
                _lasterr = errcode;
                _errstr = errstr;
                SendEvent(1);
            }
        }

        private void SendEvent(uint signal)
        {
            _handler?.Invoke(signal);
        }

    }
}
