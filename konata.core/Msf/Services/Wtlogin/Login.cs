using System;
using konata.Msf;
using Konata.Msf.Packets.Oicq;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        private Login() => name = "wtlogin.login";

        private static Login _instance = null;

        internal static Login Instance
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

        internal override bool Handle(Core core, byte[] data) => false;

        internal bool Request_TGTGT(Core core)
        {
            var request = new OicqRequestTgtgt(
                core._uin, core._password, null, null, null, null);

            var seoSeq = core._ssoMan.SendMessage(this, request);

            return false;
        }
    }
}