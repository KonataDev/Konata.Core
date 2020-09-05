using System;
using konata.Msf;
using Konata.Msf.Packets.Oicq;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        internal static bool Run(Core core, string method, params object[] args)
        {
            switch (method)
            {
                case "Request_TGTGT": return Request_TGTGT(core);
                default: return false;
            }
        }

        internal static bool Handle(Core core, byte[] data)
        {
            return false;
        }

        internal static bool Request_TGTGT(Core core)
        {
            var request = new OicqRequestTgtgt(
                core._uin, core._password, null, null, null, null);

            var seoSeq = core._ssoMan.SendSsoMessage(request);

            return false;
        }




    }
}
