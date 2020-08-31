using System;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        internal static bool Run(Core core)
        {
            return false;
        }

        internal static bool Handle(Core core, byte[] data)
        {
            return false;
        }

        internal static bool Request_TGTGT(Core core)
        {


            core._ssoMan.PutPacket();

            return false;
        }




    }
}
