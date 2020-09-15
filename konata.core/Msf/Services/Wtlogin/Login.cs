using System;
using Konata.Msf;
using Konata.Msf.Packets.Oicq;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        private static Login _instance = null;

        private Login()
        {
            name = "wtlogin.login";
        }

        public static Login Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Login();
                }
                return _instance;
            }
        }

        internal override bool Run(Core core, string method, params object[] args)
        {
            switch (method)
            {
                case "Request_TGTGT": return Request_TGTGT(core);
                default: return false;
            }
        }

        internal override bool Handle(Core core, params object[] args)
        {
            return false;
        }

        internal bool Request_TGTGT(Core core)
        {
            var sequence = core._ssoMan.GetNewSequence();

            var request = new OicqRequestTgtgt(
                core._uin, core._password, sequence, core._keyRing);

            core._ssoMan.PostMessage(this, request);

            return false;
        }
    }
}
