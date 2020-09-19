using System;
using Konata.Msf;
using Konata.Msf.Packets.Oicq;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        private Login()
        {
            Register("wtlogin.login", this);
        }

        public static Service Instance { get; } = new Login();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            switch (method)
            {
                case "Request_TGTGT": return Request_TGTGT(core);
                default: return false;
            }
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            var data = ((Packet)args[0]).GetBytes();


            return false;
        }

        /// <summary>
        /// 請求 OicqRequestTgtgt
        /// </summary>
        /// <param name="core"></param>
        internal bool Request_TGTGT(Core core)
        {
            var sequence = core._ssoMan.GetNewSequence();

            var request = new OicqRequestTgtgt(
                core._uin, core._password, sequence, core._keyRing);

            core._ssoMan.PostMessage(this, request);

            return true;
        }

        /// <summary>
        /// 請求 OicqCheckImage
        /// </summary>
        /// <param name="core"></param>
        internal bool Request_CheckImage(Core core)
        {
            var sequence = core._ssoMan.GetNewSequence();

            var request = new OicqRequestCheckImage();

            core._ssoMan.PostMessage(this, request);

            return true;
        }

        /// <summary>
        /// 處理 OicqTGTGT
        /// </summary>
        /// <param name="core"></param>
        internal bool Handle_TGTGT(Core core)
        {
            return false;
        }

    }
}
